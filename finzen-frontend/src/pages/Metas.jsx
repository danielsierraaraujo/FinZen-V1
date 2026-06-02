import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'

function Metas() {
    const [metas, setMetas] = useState([])
    const [nombre, setNombre] = useState('')
    const [descripcion, setDescripcion] = useState('')
    const [montoObjetivo, setMontoObjetivo] = useState('')
    const [prioridad, setPrioridad] = useState(1)
    const [fechaLimite, setFechaLimite] = useState('')
    const [editandoId, setEditandoId] = useState(null)
    const [excedente, setExcedente] = useState('')
    const [estrategia, setEstrategia] = useState('prioridad')
    const [resultado, setResultado] = useState(null)
    const [error, setError] = useState('')
    
    // --- NUEVOS ESTADOS PARA EL SMART ALLOCATOR ---
    const [excedenteMes, setExcedenteMes] = useState(null)
    const [cargandoExcedente, setCargandoExcedente] = useState(false)
    
    const nombre_usuario = localStorage.getItem('nombre')
    const navigate = useNavigate()

    useEffect(() => {
        cargarMetas()
    }, [])

    const cargarMetas = async () => {
        try {
            const respuesta = await api.get('/meta')
            setMetas(respuesta.data)
        } catch (err) {
            setError('Error al cargar metas')
        }
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        setError('')

        const datos = {
            nombre,
            descripcion,
            montoObjetivo: parseFloat(montoObjetivo),
            prioridad: parseInt(prioridad),
            fechaLimite
        }

        try {
            if (editandoId) {
                await api.put(`/meta/${editandoId}`, datos)
            } else {
                await api.post('/meta', datos)
            }
            limpiarFormulario()
            cargarMetas()
        } catch (err) {
            setError('Error al guardar la meta')
        }
    }

    const handleEditar = (meta) => {
        setEditandoId(meta.id)
        setNombre(meta.nombre)
        setDescripcion(meta.descripcion)
        setMontoObjetivo(meta.montoObjetivo)
        setPrioridad(meta.prioridad)
        setFechaLimite(meta.fechaLimite.split('T')[0])
    }

    const handleEliminar = async (id) => {
        if (!window.confirm('¿Eliminar esta meta?')) return
        try {
            await api.delete(`/meta/${id}`)
            cargarMetas()
        } catch (err) {
            setError('Error al eliminar')
        }
    }

    // --- NUEVA FUNCIÓN PARA CALCULAR EXCEDENTE ---
    const calcularExcedente = async () => {
        setCargandoExcedente(true)
        try {
            const respuesta = await api.get('/transaccion/excedente-mes')
            setExcedenteMes(respuesta.data)
            // Autocompleta el input con el excedente calculado
            setExcedente(respuesta.data.excedente) 
        } catch (err) {
            setError('Error al calcular excedente')
        } finally {
            setCargandoExcedente(false)
        }
    }

    const handleAsignar = async (e) => {
        e.preventDefault()
        setError('')
        setResultado(null)

        try {
            const respuesta = await api.post('/meta/asignar', {
                excedente: parseFloat(excedente),
                estrategia
            })
            setResultado(respuesta.data)
            cargarMetas()
        } catch (err) {
            setError('Error al ejecutar el algoritmo')
        }
    }

    const limpiarFormulario = () => {
        setNombre('')
        setDescripcion('')
        setMontoObjetivo('')
        setPrioridad(1)
        setFechaLimite('')
        setEditandoId(null)
    }

    const handleLogout = () => {
        localStorage.removeItem('token')
        localStorage.removeItem('nombre')
        navigate('/login')
    }

    return (
        <main className="metas-container">
            <header className="transacciones-header">
                <h1>FinZen 💰</h1>
                <section className="header-derecho">
                    <p>Hola, <strong>{nombre_usuario}</strong></p>
                    <button onClick={() => navigate('/transacciones')} className="btn-nav">
                        Transacciones
                    </button>
                    <button onClick={handleLogout} className="btn-logout">
                        Cerrar Sesión
                    </button>
                </section>
            </header>

            <section className="contenido">

                <section className="formulario-seccion">
                    <h2>{editandoId ? 'Editar Meta' : 'Nueva Meta'}</h2>

                    <form onSubmit={handleSubmit}>
                        <div className="campo-grupo">
                            <label htmlFor="nombre">Nombre</label>
                            <input
                                type="text"
                                id="nombre"
                                value={nombre}
                                onChange={(e) => setNombre(e.target.value)}
                                placeholder="Ej: Laptop, Viaje..."
                                required
                            />
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="descripcion">Descripción</label>
                            <input
                                type="text"
                                id="descripcion"
                                value={descripcion}
                                onChange={(e) => setDescripcion(e.target.value)}
                                placeholder="Descripción de la meta"
                            />
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="monto">Monto Objetivo</label>
                            <input
                                type="number"
                                id="monto"
                                value={montoObjetivo}
                                onChange={(e) => setMontoObjetivo(e.target.value)}
                                placeholder="0.00"
                                step="0.01"
                                required
                            />
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="prioridad">Prioridad (1-5)</label>
                            <select
                                id="prioridad"
                                value={prioridad}
                                onChange={(e) => setPrioridad(e.target.value)}
                            >
                                <option value={1}>1 - Baja</option>
                                <option value={2}>2 - Normal</option>
                                <option value={3}>3 - Media</option>
                                <option value={4}>4 - Alta</option>
                                <option value={5}>5 - Urgente</option>
                            </select>
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="fecha">Fecha Límite</label>
                            <input
                                type="date"
                                id="fecha"
                                value={fechaLimite}
                                onChange={(e) => setFechaLimite(e.target.value)}
                                required
                            />
                        </div>

                        {error && <p className="error-msg">{error}</p>}

                        <section className="botones-form">
                            <button type="submit" className="btn-guardar">
                                {editandoId ? 'Actualizar' : 'Guardar'}
                            </button>
                            {editandoId && (
                                <button
                                    type="button"
                                    onClick={limpiarFormulario}
                                    className="btn-cancelar"
                                >
                                    Cancelar
                                </button>
                            )}
                        </section>
                    </form>

                    <hr style={{ margin: '24px 0' }} />
                    <h2>Smart Allocator 🧠</h2>

                    <button
                        type="button"
                        onClick={calcularExcedente}
                        className="btn-calcular"
                        disabled={cargandoExcedente}
                    >
                        {cargandoExcedente ? 'Calculando...' : '📊 Calcular Excedente del Mes'}
                    </button>

                    {excedenteMes && (
                        <div className="resumen-mes">
                            <p>📅 <strong>{excedenteMes.mes}</strong></p>
                            <p>📈 Ingresos: <strong>${excedenteMes.totalIngresos}</strong></p>
                            <p>📉 Gastos: <strong>${excedenteMes.totalGastos}</strong></p>
                            <p>💰 Excedente: <strong>${excedenteMes.excedente}</strong></p>
                        </div>
                    )}

                    <form onSubmit={handleAsignar}>
                        <div className="campo-grupo">
                            <label htmlFor="excedente">Excedente a distribuir</label>
                            <input
                                type="number"
                                id="excedente"
                                value={excedente}
                                onChange={(e) => setExcedente(e.target.value)}
                                placeholder="0.00"
                                step="0.01"
                                required
                            />
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="estrategia">Estrategia de distribución</label>
                            <select
                                id="estrategia"
                                value={estrategia}
                                onChange={(e) => setEstrategia(e.target.value)}
                            >
                                <option value="prioridad">Por Prioridad</option>
                                <option value="urgencia">Por Urgencia</option>
                                <option value="equilibrada">Equilibrada</option>
                            </select>
                        </div>

                        <button type="submit" className="btn-guardar">
                            🚀 Ejecutar Smart Allocator
                        </button>
                    </form>

                    {resultado && (
                        <section className="resultado-algoritmo">
                            <h3>Resultado — {resultado.estrategiaUsada}</h3>
                            <p>Excedente distribuido: <strong>${resultado.excedenteTotal}</strong></p>
                            <ul>
                                {resultado.asignaciones.map((a) => (
                                    <li key={a.metaId}>
                                        <strong>{a.nombreMeta}</strong>
                                        <span> → +${a.montoAsignado}</span>
                                        <span> ({a.porcentajeCompletadoAntes}% → {a.porcentajeCompletadoDespues}%)</span>
                                    </li>
                                ))}
                            </ul>
                        </section>
                    )}
                </section>

                <section className="lista-seccion">
                    <h2>Mis Metas</h2>
                    {metas.length === 0 ? (
                        <p className="sin-transacciones">No tienes metas aún</p>
                    ) : (
                        <ul className="lista-metas">
                            {metas.map((meta) => (
                                <li key={meta.id} className="meta-item">
                                    <section className="meta-info">
                                        <strong>{meta.nombre}</strong>
                                        <span className="categoria">{meta.descripcion}</span>
                                        <span>Prioridad: {meta.prioridad}/5</span>
                                        <span>{meta.diasRestantes} días restantes</span>
                                    </section>

                                    <section className="meta-progreso">
                                        <div className="barra-fondo">
                                            <div
                                                className="barra-progreso"
                                                style={{ width: `${Math.min(meta.porcentajeCompletado, 100)}%` }}
                                            />
                                        </div>
                                        <span>${meta.montoActual} / ${meta.montoObjetivo}</span>
                                        <span>{Math.round(meta.porcentajeCompletado)}%</span>
                                    </section>

                                    <section className="transaccion-botones">
                                        <button
                                            onClick={() => handleEditar(meta)}
                                            className="btn-editar"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            onClick={() => handleEliminar(meta.id)}
                                            className="btn-eliminar"
                                        >
                                            Eliminar
                                        </button>
                                    </section>
                                </li>
                            ))}
                        </ul>
                    )}
                </section>

            </section>
        </main>
    )
}

export default Metas