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


## 🎯 Cumplimiento de Requisitos (Administración Core)

Este proyecto cumple con los lineamientos solicitados para la administración del core:

1. **Validación Back-End (Dato Sensible):** La contraseña de los usuarios (dato crítico) es validada y procesada estrictamente en el backend mediante el `AuthService` y `AuthController` antes de interactuar con la base de datos. La seguridad no depende del lado del cliente (JS).
2. **Relación de Tablas (Dropdowns Dinámicos):** Al ingresar una nueva transacción, la llave foránea (`CategoriaId`) no se ingresa manualmente. El formulario cuenta con dropdowns relacionados: al seleccionar el "Tipo" de transacción (Ingreso/Gasto), el sistema realiza una petición al backend y carga dinámicamente el dropdown de "Categorías" mostrando únicamente las opciones pertenecientes a ese tipo.
3. **Versionado y Deploy:** Código versionado correctamente en Git y aplicación funcional desplegada en la nube con este archivo README estructurado.

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
- ✅ Clasificación de transacciones por tipo (Ingreso/Gasto) con carga dinámica de categorías.
- ✅ Cierre de sesión

## 🏗️ Arquitectura

El proyecto sigue el patrón **MVC (Modelo - Vista - Controlador)**:

FinZen/
├── FinZen.API/                        → Backend (.NET Core)
│   ├── Controllers/                   → Reciben peticiones HTTP
│   │   ├── AuthController.cs          → Login y registro
│   │   ├── TransaccionController.cs   → CRUD de transacciones
│   │   └── CategoriaController.cs     → Carga dinámica de categorías
│   ├── Models/                        → Representan las tablas
│   │   ├── Usuario.cs
│   │   ├── Transaccion.cs
│   │   └── Categoria.cs               → Tabla relacional
│   ├── Data/
│   │   └── AppDbContext.cs            → Contexto de BD y Data Seeding
│   ├── Services/
│   │   └── AuthService.cs             → Lógica de negocio y encriptación
│   ├── DTOs/
│   │   ├── AuthDTOs.cs
│   │   └── TransaccionDTOs.cs
│   └── Program.cs                     → Punto de entrada
│
└── finzen-frontend/                   → Frontend (React)
    └── src/
        ├── pages/
        │   ├── Login.jsx
        │   └── Transacciones.jsx      → Contiene la lógica de dropdowns dependientes
        ├── components/
        │   └── ProtectedRoute.jsx
        ├── services/
        │   └── api.js
        └── App.jsx

## 👨‍💻 Autor

Desarrollado por **Daniel Sierra** — Materia de Ingeniería Web