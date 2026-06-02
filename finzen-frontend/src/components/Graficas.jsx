import { Pie, Bar } from 'react-chartjs-2'
import {
    Chart as ChartJS,
    ArcElement,
    Tooltip,
    Legend,
    CategoryScale,
    LinearScale,
    BarElement,
    Title
} from 'chart.js'

ChartJS.register(
    ArcElement,
    Tooltip,
    Legend,
    CategoryScale,
    LinearScale,
    BarElement,
    Title
)

function Graficas({ transacciones, excedenteMes }) {

    // Agrupar gastos por categoría
    const gastosPorCategoria = transacciones
        .filter(t => t.tipo === 'Gasto')
        .reduce((acc, t) => {
            acc[t.categoriaNombre] = (acc[t.categoriaNombre] || 0) + t.monto
            return acc
        }, {})

    const pieData = {
        labels: Object.keys(gastosPorCategoria),
        datasets: [{
            data: Object.values(gastosPorCategoria),
            backgroundColor: [
                '#2563eb', '#16a34a', '#dc2626',
                '#d97706', '#7c3aed', '#db2777',
                '#0891b2', '#65a30d'
            ],
            borderWidth: 2,
            borderColor: 'white'
        }]
    }

    const barData = {
        labels: ['Este mes'],
        datasets: [
            {
                label: 'Ingresos',
                data: [excedenteMes?.totalIngresos || 0],
                backgroundColor: '#16a34a',
                borderRadius: 8
            },
            {
                label: 'Gastos',
                data: [excedenteMes?.totalGastos || 0],
                backgroundColor: '#dc2626',
                borderRadius: 8
            }
        ]
    }

    const barOptions = {
        responsive: true,
        plugins: {
            legend: { position: 'top' },
            title: {
                display: true,
                text: 'Ingresos vs Gastos'
            }
        }
    }

    const pieOptions = {
        responsive: true,
        plugins: {
            legend: { position: 'bottom' },
            title: {
                display: true,
                text: 'Gastos por Categoría'
            }
        }
    }

    return (
        <section className="graficas-container">
            <h2>📊 Resumen del Mes</h2>
            <div className="graficas-grid">
                <div className="grafica-card">
                    {Object.keys(gastosPorCategoria).length > 0 ? (
                        <Pie data={pieData} options={pieOptions} />
                    ) : (
                        <p className="sin-transacciones">
                            No hay gastos este mes
                        </p>
                    )}
                </div>
                <div className="grafica-card">
                    <Bar data={barData} options={barOptions} />
                </div>
            </div>
        </section>
    )
}

export default Graficas