```markdown
# FinZen

Aplicación web de finanzas personales para registrar ingresos y gastos, desarrollada como proyecto de la materia de Ingeniería Web.

##  Tecnologías utilizadas

- **Backend:** .NET Core 10 - Web API
- **Frontend:** React + Vite
- **Base de datos:** PostgreSQL
- **ORM:** Entity Framework Core
- **Autenticación:** JWT (JSON Web Tokens)
- **Estilos:** CSS puro

##  Funcionalidades

- Registro e inicio de sesión con JWT
- Rutas protegidas (no accesibles sin autenticación)
- CRUD completo de transacciones
- Clasificación de transacciones por tipo (Ingreso/Gasto)
- Clasificación por categoría
- Cierre de sesión

##  Arquitectura

El proyecto sigue el patrón **MVC (Modelo - Vista - Controlador)**:

```
FinZen/
├── FinZen.API/                  → Backend (.NET Core)
│   ├── Controllers/             → Controladores (reciben peticiones HTTP)
│   │   ├── AuthController.cs    → Login y registro
│   │   └── TransaccionController.cs → CRUD de transacciones
│   ├── Models/                  → Modelos (representan las tablas)
│   │   ├── Usuario.cs
│   │   └── Transaccion.cs
│   ├── Data/                    → Contexto de base de datos
│   │   └── AppDbContext.cs
│   ├── Services/                → Lógica de negocio
│   │   └── AuthService.cs
│   ├── DTOs/                    → Objetos de transferencia de datos
│   │   ├── AuthDTOs.cs
│   │   └── TransaccionDTOs.cs
│   └── Program.cs               → Punto de entrada y configuración
│
└── finzen-frontend/             → Frontend (React)
    └── src/
        ├── pages/               → Páginas de la app
        │   ├── Login.jsx
        │   └── Transacciones.jsx
        ├── components/          → Componentes reutilizables
        │   └── ProtectedRoute.jsx
        ├── services/            → Comunicación con la API
        │   └── api.js
        └── App.jsx              → Rutas principales