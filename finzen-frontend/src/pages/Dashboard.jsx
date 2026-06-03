import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../services/api'
import Graficas from '../components/Graficas'

// 1. IMPORTAMOS EL NUEVO COMPONENTE
import TopUsuariosRanking from '../components/TopUsuariosRanking'

function Dashboard() {
    const [excedenteMes, setExcedenteMes] = useState(null)
    const [metas, setMetas] = useState([])
    const [transacciones, setTransacciones] = useState([])
    const nombre = localStorage.getItem('nombre')
    const navigate = useNavigate()

    useEffect(() => {
        cargarDatos()
    }, [])

    const cargarDatos = async () => {
        try {
            const [excedente, metasRes, transRes] = await Promise.all([
                api.get('/transaccion/excedente-mes'),
                api.get('/meta'),
                api.get('/transaccion')
            ])
            setExcedenteMes(excedente.data)
            setMetas(metasRes.data)
            setTransacciones(transRes.data)
        } catch (err) {
            console.log('Error al cargar datos')
        }
    }

    const handleLogout = () => {
        localStorage.removeItem('token')
        localStorage.removeItem('nombre')
        navigate('/login')
    }

    const metasUrgentes = metas
        .filter(m => m.porcentajeCompletado < 100)
        .sort((a, b) => a.diasRestantes - b.diasRestantes)
        .slice(0, 3)

    return (
        <main className="dashboard-container">

            {/* HEADER */}
            <header className="transacciones-header">
                <h1>FinZen 💰</h1>
                <section className="header-derecho">
                    <button onClick={() => navigate('/transacciones')} className="btn-nav">
                        Transacciones
                    </button>
                    <button onClick={() => navigate('/metas')} className="btn-nav">
                        Metas
                    </button>
                    <button onClick={handleLogout} className="btn-logout">
                        Cerrar Sesión
                    </button>
                </section>
            </header>

            {/* BIENVENIDA */}
            <section className="dashboard-bienvenida">
                <h2>Bienvenido, <span>{nombre}</span> 👋</h2>
                <p>Aquí tienes el resumen de tus finanzas este mes</p>
            </section>

            {/* TARJETAS DE RESUMEN */}
            {excedenteMes && (
                <section className="dashboard-cards">
                    <article className="dashboard-card ingreso">
                        <p className="card-label">📈 Total Ingresos</p>
                        <p className="card-valor">${excedenteMes.totalIngresos.toFixed(2)}</p>
                    </article>
                    <article className="dashboard-card gasto">
                        <p className="card-label">📉 Total Gastos</p>
                        <p className="card-valor">${excedenteMes.totalGastos.toFixed(2)}</p>
                    </article>
                    <article className="dashboard-card excedente">
                        <p className="card-label">💰 Excedente</p>
                        <p className="card-valor">${excedenteMes.excedente.toFixed(2)}</p>
                    </article>
                    <article className="dashboard-card metas">
                        <p className="card-label">🎯 Metas Activas</p>
                        <p className="card-valor">{metas.filter(m => m.porcentajeCompletado < 100).length}</p>
                    </article>
                </section>
            )}

            {/* CONTENEDOR FLEX PARA METAS URGENTES + RANKING */}
            <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap', marginBottom: '30px' }}>
                
                {/* LADO IZQUIERDO: METAS MÁS URGENTES (Tu código original) */}
                <section className="dashboard-metas" style={{ flex: '1', minWidth: '300px', margin: '0' }}>
                    <h3>🔥 Metas más urgentes</h3>
                    {metasUrgentes.length === 0 ? (
                        <p className="sin-transacciones">No tienes metas activas</p>
                    ) : (
                        <ul className="lista-metas">
                            {metasUrgentes.map((meta) => (
                                <li key={meta.id} className="meta-item">
                                    <section className="meta-info">
                                        <strong>{meta.nombre}</strong>
                                        <span className="categoria">
                                            {meta.diasRestantes} días restantes
                                        </span>
                                    </section>
                                    <section className="meta-progreso">
                                        <div className="barra-fondo">
                                            <div
                                                className="barra-progreso"
                                                style={{width: `${Math.min(meta.porcentajeCompletado, 100)}%`}}
                                            />
                                        </div>
                                        <span>${meta.montoActual} / ${meta.montoObjetivo}</span>
                                        <span>{Math.round(meta.porcentajeCompletado)}%</span>
                                    </section>
                                </li>
                            ))}
                        </ul>
                    )}
                    <button
                        onClick={() => navigate('/metas')}
                        className="btn-ver-metas"
                        style={{ marginTop: '15px' }}
                    >
                        Ver todas las metas →
                    </button>
                </section>

                {/* LADO DERECHO: CUADRO DE HONOR (Gamificación) */}
                <section style={{ flex: '1', minWidth: '300px' }}>
                    <TopUsuariosRanking />
                </section>

            </div>

            {/* GRÁFICAS */}
            <Graficas
                transacciones={transacciones}
                excedenteMes={excedenteMes}
            />

        </main>
    )
}

export default Dashboard