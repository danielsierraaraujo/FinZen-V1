import axios from 'axios'

const api = axios.create({
    // Le quitamos la variable y le ponemos tu servidor real de Railway
    baseURL: 'https://finzen-v1-production.up.railway.app'
})

api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token')
    if (token) {
        config.headers.Authorization = `Bearer ${token}`
    }
    return config
})

export default api