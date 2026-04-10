import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'

function Transacciones() {
    const [transacciones, setTransacciones] = useState([])
    const [descripcion, setDescripcion] = useState('')
    const [monto, setMonto] = useState('')
    const [tipo, setTipo] = useState('Gasto')
    const [categoria, setCategoria] = useState('')
    const [editandoId, setEditandoId] = useState(null)
    const [error, setError] = useState('')
    const nombre = localStorage.getItem('nombre')
    const navigate = useNavigate()

    useEffect(() => {
        cargarTransacciones()
    }, [])

    const cargarTransacciones = async () => {
        try {
            const respuesta = await api.get('/transaccion')
            setTransacciones(respuesta.data)
        } catch (err) {
            setError('Error al cargar transacciones')
        }
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        setError('')

        const datos = { descripcion, monto: parseFloat(monto), tipo, categoria }

        try {
            if (editandoId) {
                await api.put(`/transaccion/${editandoId}`, datos)
            } else {
                await api.post('/transaccion', datos)
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
        setCategoria(transaccion.categoria)
    }

    const handleEliminar = async (id) => {
        if (!window.confirm('¿Seguro que deseas eliminar esta transacción?')) return
        try {
            await api.delete(`/transaccion/${id}`)
            cargarTransacciones()
        } catch (err) {
            setError('Error al eliminar')
        }
    }

    const limpiarFormulario = () => {
        setDescripcion('')
        setMonto('')
        setTipo('Gasto')
        setCategoria('')
        setEditandoId(null)
    }

    const handleLogout = () => {
        localStorage.removeItem('token')
        localStorage.removeItem('nombre')
        navigate('/login')
    }

    return (
        <main className="transacciones-container">
            <header className="transacciones-header">
                <h1>FinZen 💰</h1>
                <section className="header-derecho">
                    <p>Hola, <strong>{nombre}</strong></p>
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
                                onChange={(e) => setTipo(e.target.value)}
                            >
                                <option value="Gasto">Gasto</option>
                                <option value="Ingreso">Ingreso</option>
                            </select>
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="categoria">Categoría</label>
                            <input
                                type="text"
                                id="categoria"
                                value={categoria}
                                onChange={(e) => setCategoria(e.target.value)}
                                placeholder="Ej: Comida, Salario..."
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
                                        <span className="categoria">{t.categoria}</span>
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
        </main>
    )
}

export default Transacciones