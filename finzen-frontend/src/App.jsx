import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import Login from './pages/Login'
import Transacciones from './pages/Transacciones'
import Metas from './pages/Metas'
import ProtectedRoute from './components/ProtectedRoute'

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Navigate to="/login" />} />
                <Route path="/login" element={<Login />} />
                <Route
                    path="/transacciones"
                    element={
                        <ProtectedRoute>
                            <Transacciones />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/metas"
                    element={
                        <ProtectedRoute>
                            <Metas />
                        </ProtectedRoute>
                    }
                />
            </Routes>
        </BrowserRouter>
    )
}

export default App