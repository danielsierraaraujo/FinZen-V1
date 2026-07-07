import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import api from '../services/api'

function Register() {
    const [nombre, setNombre] = useState('')
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState('')
    const [cargando, setCargando] = useState(false)
    const navigate = useNavigate()

    const handleSubmit = async (e) => {
        e.preventDefault()
        setCargando(true)
        setError('')

        try {
            const respuesta = await api.post('/api/auth/register', { nombre, email, password })
            localStorage.setItem('token', respuesta.data.token)
            localStorage.setItem('nombre', respuesta.data.nombre)
            navigate('/transacciones')
        } catch (err) {
            setError(err.response?.data?.mensaje || 'No se pudo completar el registro')
        } finally {
            setCargando(false)
        }
    }

    return (
        <main className="login-container">
            <section className="login-box">
                <header className="login-header">
                    <h1>FinZen 💰</h1>
                    <p>Crea tu cuenta</p>
                </header>

                <form onSubmit={handleSubmit}>
                    <div className="campo-grupo">
                        <label htmlFor="nombre">Nombre</label>
                        <input
                            type="text"
                            id="nombre"
                            value={nombre}
                            onChange={(e) => setNombre(e.target.value)}
                            placeholder="Tu nombre"
                            required
                        />
                    </div>

                    <div className="campo-grupo">
                        <label htmlFor="email">Email</label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="tu@email.com"
                            required
                        />
                    </div>

                    <div className="campo-grupo">
                        <label htmlFor="password">Contraseña</label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Mínimo 8 caracteres, 1 mayúscula y 1 número"
                            required
                        />
                    </div>

                    {error && <p className="error-msg">{error}</p>}

                    <button type="submit" disabled={cargando}>
                        {cargando ? 'Creando cuenta...' : 'Crear cuenta'}
                    </button>
                </form>

                <p className="link-msg">
                    ¿Ya tienes cuenta? <Link to="/login">Inicia sesión</Link>
                </p>
            </section>
        </main>
    )
}

export default Register
