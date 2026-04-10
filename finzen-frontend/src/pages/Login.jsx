import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'

function Login() {
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
            const respuesta = await api.post('/auth/login', { email, password })
            localStorage.setItem('token', respuesta.data.token)
            localStorage.setItem('nombre', respuesta.data.nombre)
            navigate('/transacciones')
        } catch (err) {
            setError('Email o contraseña incorrectos')
        } finally {
            setCargando(false)
        }
    }

    return (
        <main className="login-container">
            <section className="login-box">
                <header className="login-header">
                    <h1>FinZen 💰</h1>
                    <p>Inicia sesión en tu cuenta</p>
                </header>

                <form onSubmit={handleSubmit}>
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
                            placeholder="Tu contraseña"
                            required
                        />
                    </div>

                    {error && <p className="error-msg">{error}</p>}

                    <button type="submit" disabled={cargando}>
                        {cargando ? 'Ingresando...' : 'Iniciar Sesión'}
                    </button>
                </form>
            </section>
        </main>
    )
}

export default Login