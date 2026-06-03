import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'
import Graficas from '../components/Graficas'

function Transacciones() {
    const [transacciones, setTransacciones] = useState([])
    const [categorias, setCategorias] = useState([])
    const [descripcion, setDescripcion] = useState('')
    const [monto, setMonto] = useState('')
    const [tipo, setTipo] = useState('Gasto')
    const [categoriaId, setCategoriaId] = useState('')
    const [editandoId, setEditandoId] = useState(null)
    const [error, setError] = useState('')
    const nombre = localStorage.getItem('nombre')
    const navigate = useNavigate()
    const [excedenteMes, setExcedenteMes] = useState(null)

    useEffect(() => {
        cargarTransacciones()
        cargarCategorias()
        cargarExcedente()
    }, [])

    const cargarTransacciones = async () => {
        try {
            const respuesta = await api.get('/api/transaccion')
            setTransacciones(respuesta.data)
        } catch (err) {
            setError('Error al cargar transacciones')
        }
    }

    const cargarCategorias = async () => {
        try {
            const respuesta = await api.get('/api/categoria')
            setCategorias(respuesta.data)
        } catch (err) {
            setError('Error al cargar categorías')
        }
    }

    const categoriasFiltradas = categorias.filter(c =>
        c.tipo === tipo || c.tipo === 'Ambos'
    )

    const handleSubmit = async (e) => {
        e.preventDefault()
        setError('')

        const datos = {
            descripcion,
            monto: parseFloat(monto),
            tipo,
            categoriaId: parseInt(categoriaId)
        }

        try {
            if (editandoId) {
                await api.put(`/api/transaccion/${editandoId}`, datos)
            } else {
                await api.post('/api/transaccion', datos)
            }
            limpiarFormulario()
            cargarTransacciones()
        } catch (err) {
            setError('Error al guardar la transacción')
        }
    }

    const handleEditar = (transaccion) => {
        setEditandoId(transaccion.id)
        setDescripcion(transaccion.descripcion)
        setMonto(transaccion.monto)
        setTipo(transaccion.tipo)
        setCategoriaId(transaccion.categoriaId)
    }

    const handleEliminar = async (id) => {
        if (!window.confirm('¿Seguro que deseas eliminar?')) return
        try {
            await api.delete(`/api/transaccion/${id}`)
            cargarTransacciones()
        } catch (err) {
            setError('Error al eliminar')
        }
    }

    const limpiarFormulario = () => {
        setDescripcion('')
        setMonto('')
        setTipo('Gasto')
        setCategoriaId('')
        setEditandoId(null)
    }

    const handleLogout = () => {
        localStorage.removeItem('token')
        localStorage.removeItem('nombre')
        navigate('/login')
    }

    const cargarExcedente = async () => {
        try {
            const respuesta = await api.get('/api/transaccion/excedente-mes')
            setExcedenteMes(respuesta.data)
        } catch (err) {
            console.log('Error al cargar excedente')
        }
    }

    return (
        <main className="transacciones-container">
            <header className="transacciones-header">
    {/* Si le haces clic al logo, también te lleva al dashboard (súper común en diseño web) */}
    <h1 style={{ cursor: 'pointer' }} onClick={() => navigate('/dashboard')}>
        FinZen 💰
    </h1>
    
    <section className="header-derecho">
        {/* NUEVO BOTÓN PARA IR AL DASHBOARD */}
        <button onClick={() => navigate('/dashboard')} className="btn-nav">
            Dashboard
        </button>
        
        <button onClick={() => navigate('/metas')} className="btn-nav">
            Metas
        </button>
        <button onClick={handleLogout} className="btn-logout">
            Cerrar Sesión
        </button>
    </section>
</header>

            <section className="contenido">
                <section className="formulario-seccion">
                    <h2>{editandoId ? 'Editar Transacción' : 'Nueva Transacción'}</h2>

                    <form onSubmit={handleSubmit}>
                        <div className="campo-grupo">
                            <label htmlFor="descripcion">Descripción</label>
                            <input
                                type="text"
                                id="descripcion"
                                value={descripcion}
                                onChange={(e) => setDescripcion(e.target.value)}
                                placeholder="Ej: Almuerzo, Sueldo..."
                                required
                            />
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="monto">Monto</label>
                            <input
                                type="number"
                                id="monto"
                                value={monto}
                                onChange={(e) => setMonto(e.target.value)}
                                placeholder="0.00"
                                step="0.01"
                                required
                            />
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="tipo">Tipo</label>
                            <select
                                id="tipo"
                                value={tipo}
                                onChange={(e) => {
                                    setTipo(e.target.value)
                                    setCategoriaId('')
                                }}
                            >
                                <option value="Gasto">Gasto</option>
                                <option value="Ingreso">Ingreso</option>
                            </select>
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="categoria">Categoría</label>
                            <select
                                id="categoria"
                                value={categoriaId}
                                onChange={(e) => setCategoriaId(e.target.value)}
                                required
                            >
                                <option value="">Selecciona una categoría</option>
                                {categoriasFiltradas.map((cat) => (
                                    <option key={cat.id} value={cat.id}>
                                        {cat.nombre}
                                    </option>
                                ))}
                            </select>
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
                </section>

                <section className="lista-seccion">
                    <h2>Mis Transacciones</h2>
                    {transacciones.length === 0 ? (
                        <p className="sin-transacciones">
                            No tienes transacciones aún
                        </p>
                    ) : (
                        <ul className="lista-transacciones">
                            {transacciones.map((t) => (
                                <li key={t.id} className={`transaccion-item ${t.tipo.toLowerCase()}`}>
                                    <section className="transaccion-info">
                                        <strong>{t.descripcion}</strong>
                                        <span className="categoria">{t.categoriaNombre}</span>
                                    </section>
                                    <section className="transaccion-derecha">
                                        <span className="monto">
                                            {t.tipo === 'Gasto' ? '-' : '+'} ${t.monto}
                                        </span>
                                        <section className="transaccion-botones">
                                            <button
                                                onClick={() => handleEditar(t)}
                                                className="btn-editar"
                                            >
                                                Editar
                                            </button>
                                            <button
                                                onClick={() => handleEliminar(t.id)}
                                                className="btn-eliminar"
                                            >
                                                Eliminar
                                            </button>
                                        </section>
                                    </section>
                                </li>
                            ))}
                        </ul>
                    )}
                </section>
            </section>

            <Graficas
                transacciones={transacciones}
                excedenteMes={excedenteMes}
            />
        </main>
    )
}

export default Transacciones