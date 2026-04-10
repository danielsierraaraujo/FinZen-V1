# FinZen 💰

Aplicación web de finanzas personales para registrar ingresos y gastos, desarrollada como proyecto de la materia de Ingeniería Web.

## 🛠️ Tecnologías utilizadas

- **Backend:** .NET Core 10 - Web API
- **Frontend:** React + Vite
- **Base de datos:** PostgreSQL
- **ORM:** Entity Framework Core
- **Autenticación:** JWT (JSON Web Tokens)
- **Estilos:** CSS puro

## 📋 Funcionalidades

- ✅ Registro e inicio de sesión con JWT
- ✅ Rutas protegidas (no accesibles sin autenticación)
- ✅ CRUD completo de transacciones
- ✅ Clasificación de transacciones por tipo (Ingreso/Gasto)
- ✅ Clasificación por categoría
- ✅ Cierre de sesión

## 🏗️ Arquitectura

El proyecto sigue el patrón **MVC (Modelo - Vista - Controlador)**:

FinZen/
├── FinZen.API/                        → Backend (.NET Core)
│   ├── Controllers/                   → Reciben peticiones HTTP
│   │   ├── AuthController.cs          → Login y registro
│   │   └── TransaccionController.cs   → CRUD de transacciones
│   ├── Models/                        → Representan las tablas
│   │   ├── Usuario.cs
│   │   └── Transaccion.cs
│   ├── Data/
│   │   └── AppDbContext.cs            → Contexto de base de datos
│   ├── Services/
│   │   └── AuthService.cs            → Lógica de negocio
│   ├── DTOs/
│   │   ├── AuthDTOs.cs
│   │   └── TransaccionDTOs.cs
│   └── Program.cs                    → Punto de entrada
│
└── finzen-frontend/                  → Frontend (React)
└── src/
├── pages/
│   ├── Login.jsx
│   └── Transacciones.jsx
├── components/
│   └── ProtectedRoute.jsx
├── services/
│   └── api.js
└── App.jsx

## ⚙️ Instalación y configuración

### Requisitos previos
- .NET SDK 10
- Node.js 20+
- PostgreSQL 18

### 1. Clonar el repositorio

git clone https://github.com/danielsierraaraujo/FinZen-V1.git
cd FinZen

### 2. Configurar la base de datos
Crear una base de datos en PostgreSQL llamada `finzen` y actualizar las credenciales en `FinZen.API/appsettings.json`:

Host=localhost;Port=5432;Database=finzen;Username=postgres;Password=TU_PASSWORD

### 3. Correr el backend

cd FinZen.API
dotnet ef database update
dotnet run  
El backend corre en `http://localhost:5230`

### 4. Correr el frontend

cd finzen-frontend
npm install
npm run dev
El frontend corre en `http://localhost:5173`

## 🔐 Endpoints de la API

### Autenticación
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | /api/auth/register | Registrar usuario |
| POST | /api/auth/login | Iniciar sesión |

### Transacciones (requieren token JWT)
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | /api/transaccion | Obtener todas las transacciones |
| POST | /api/transaccion | Crear transacción |
| PUT | /api/transaccion/{id} | Actualizar transacción |
| DELETE | /api/transaccion/{id} | Eliminar transacción |

## 👨‍💻 Autor

Desarrollado por **Daniel** — Ingeniería Web