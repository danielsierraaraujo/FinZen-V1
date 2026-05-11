import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'

function Transacciones() {
    const [transacciones, setTransacciones] = useState([])
    const [categoriasLista, setCategoriasLista] = useState([]) // NUEVO: Estado para guardar las categorías de la BD
    const [descripcion, setDescripcion] = useState('')
    const [monto, setMonto] = useState('')
    const [tipo, setTipo] = useState('Gasto')
    const [categoriaId, setCategoriaId] = useState('') // MODIFICADO: Ahora guardamos el ID, no el texto
    const [editandoId, setEditandoId] = useState(null)
    const [error, setError] = useState('')
    const nombre = localStorage.getItem('nombre')
    const navigate = useNavigate()

    useEffect(() => {
        cargarTransacciones()
        cargarCategorias() // NUEVO: Cargar categorías al inicio
    }, [])

    const cargarTransacciones = async () => {
        try {
            const respuesta = await api.get('/transaccion')
            setTransacciones(respuesta.data)
        } catch (err) {
            setError('Error al cargar transacciones')
        }
    }

    // NUEVO: Función para traer las categorías desde el backend
    const cargarCategorias = async () => {
        try {
            const respuesta = await api.get('/categoria')
            setCategoriasLista(respuesta.data)
        } catch (err) {
            console.error('Error al cargar categorías', err)
        }
    }

    // NUEVO: Lógica que filtra las categorías dependiendo si es Gasto o Ingreso (y Ambos)
    const categoriasFiltradas = categoriasLista.filter(
        c => c.tipo === tipo || c.tipo === 'Ambos'
    )

    // NUEVO: Cuando el usuario cambia el tipo, limpiamos la categoría seleccionada
    const handleTipoChange = (e) => {
        setTipo(e.target.value)
        setCategoriaId('') 
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        setError('')

        // MODIFICADO: El backend ahora espera "categoriaId" como número
        const datos = { 
            descripcion, 
            monto: parseFloat(monto), 
            tipo, 
            categoriaId: parseInt(categoriaId) 
        }

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
        
        // MODIFICADO: Buscamos el ID de la categoría basándonos en el nombre que devuelve el backend
        const catEncontrada = categoriasLista.find(c => c.nombre === transaccion.categoriaNombre)
        setCategoriaId(catEncontrada ? catEncontrada.id : '')
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
        setCategoriaId('') // MODIFICADO
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
                                onChange={handleTipoChange} /* MODIFICADO: Usa la nueva función */
                            >
                                <option value="Gasto">Gasto</option>
                                <option value="Ingreso">Ingreso</option>
                            </select>
                        </div>

                        <div className="campo-grupo">
                            <label htmlFor="categoria">Categoría</label>
                            {/* MODIFICADO: Pasó de ser un <input> de texto a un <select> dinámico */}
                            <select
                                id="categoria"
                                value={categoriaId}
                                onChange={(e) => setCategoriaId(e.target.value)}
                                disabled={!tipo}
                                required
                            >
                                <option value="" disabled>Selecciona una categoría...</option>
                                {categoriasFiltradas.map(c => (
                                    <option key={c.id} value={c.id}>
                                        {c.nombre}
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
                                        {/* MODIFICADO: El DTO ahora devuelve categoriaNombre */}
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
        </main>
    )
}

export default Transacciones