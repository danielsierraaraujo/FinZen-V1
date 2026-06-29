import React, { useState, useEffect } from 'react';

const TopUsuariosRanking = () => {
    const [topUsuarios, setTopUsuarios] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchTopUsuarios = async () => {
            try {
                // Asegúrate de poner tu token de autorización si el endpoint lo requiere
                const token = localStorage.getItem('token'); 
                const response = await fetch('https://finzen-v1-production.up.railway.app/api/reporte/top-usuarios', {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });
                
                if (response.ok) {
                    const data = await response.json();
                    setTopUsuarios(data);
                }
            } catch (error) {
                console.error("Error al cargar el ranking:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchTopUsuarios();
    }, []);

    if (loading) return <p>Cargando el Cuadro de Honor...</p>;

    return (
        <div style={{ padding: '20px', border: '1px solid #ccc', borderRadius: '10px', maxWidth: '400px', backgroundColor: '#f9f9f9' }}>
            <h3 style={{ textAlign: 'center', color: '#333' }}>🏆 Cuadro de Honor FinZen</h3>
            <p style={{ textAlign: 'center', fontSize: '12px', color: '#666' }}>Usuarios con mayor anticipación en sus metas</p>
            
            <ul style={{ listStyleType: 'none', padding: 0 }}>
                {topUsuarios.map((usuario, index) => (
                    <li key={index} style={{ 
                        display: 'flex', 
                        justifyContent: 'space-between', 
                        alignItems: 'center',
                        padding: '10px', 
                        borderBottom: '1px solid #eee',
                        backgroundColor: index === 0 ? '#fffbea' : 'transparent' // Resalta al ganador en amarillo
                    }}>
                        <div>
                            <span style={{ fontSize: '24px', marginRight: '10px' }}>{medallas[index]}</span>
                            <strong>{usuario.nombreUsuario}</strong>
                            <div style={{ fontSize: '12px', color: '#888', marginLeft: '35px' }}>
                                {usuario.metasCompletadas} metas logradas
                            </div>
                        </div>
                        <div style={{ textAlign: 'right', color: '#2ecc71', fontWeight: 'bold' }}>
                            {usuario.promedioDiasAdelantado} días <br/>
                            <span style={{ fontSize: '10px', color: '#999' }}>de anticipación</span>
                        </div>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default TopUsuariosRanking;