using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FinZen.API.Data;
using FinZen.API.Services;
using FinZen.API.Interfaces;
using FinZen.API.Repositories;
using FinZen.API.Factories;

var builder = WebApplication.CreateBuilder(args);

// 1. Base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Servicios
builder.Services.AddScoped<AuthService>();

// 2.1 Repositorios (Repository Pattern)
builder.Services.AddScoped<IMetaRepository, MetaRepository>();
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();

// 2.2 Estrategias de asignación registradas por clave (Strategy + Factory Pattern).
// Agregar una nueva estrategia solo requiere una línea aquí: no exige tocar el Factory ni el controlador (OCP).
builder.Services.AddKeyedScoped<IEstrategiaAsignacion, EstrategiaPorPrioridad>("prioridad");
builder.Services.AddKeyedScoped<IEstrategiaAsignacion, EstrategiaPorUrgencia>("urgencia");
builder.Services.AddKeyedScoped<IEstrategiaAsignacion, EstrategiaEquilibrada>("equilibrada");
builder.Services.AddScoped<IEstrategiaAsignacionFactory, EstrategiaAsignacionFactory>();

// 3. Autenticación JWT
var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
});

// 4. CORS (¡Actualizado para permitir Vercel!)
// 4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FinZenPolicy", policy =>
{
    policy.AllowAnyOrigin()    // Esto deja entrar a CUALQUIER dominio de Vercel
          .AllowAnyHeader()
          .AllowAnyMethod();
});
});

// 5. Controladores
builder.Services.AddControllers();

var app = builder.Build();

// 6. Middlewares (El orden aquí está perfecto)
app.UseCors("FinZenPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 7. Migraciones automáticas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinZen.API.Data.AppDbContext>();
    db.Database.Migrate();
}

app.Run();