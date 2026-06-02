using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FinZen.API.Data;
using FinZen.API.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Servicios
builder.Services.AddScoped<AuthService>();

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
        policy.WithOrigins(
                "http://localhost:5173", // Para tus pruebas locales
                "https://fin-zen-v1-fbk8xyjat-danielsierraaraujos-projects.vercel.app" // Tu URL real de Vercel
            )
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