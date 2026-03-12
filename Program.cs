using InventarioAPI.Data;
using InventarioAPI.Helpers;
using InventarioAPI.Services;
using InventarioAPI.Services.Implementations;
using InventarioAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Scalar.AspNetCore; // Nueva librería para ver la API

var builder = WebApplication.CreateBuilder(args);

// ---------------- 1. BASE DE DATOS ----------------
builder.Services.AddDbContext<InventarioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------------- 2. DOCUMENTACIÓN (SCALAR/OPENAPI) ----------------
builder.Services.AddOpenApi(); // Configuración nativa de .NET 10

// ---------------- 3. SERVICIOS ----------------
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// ---------------- 4. JWT ----------------
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ClaveTemporalDe32CaracteresMinimo123";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

var app = builder.Build();

// ----------- 5. MIDDLEWARE (DOCUMENTACIÓN MODERNA) -----------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Genera el JSON de la API
    app.MapScalarApiReference(); // Muestra la interfaz bonita para probar
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 🔥 6. INICIALIZADOR
using (var scope = app.Services.CreateScope())
{
    DbInitializer.Inicializar(scope.ServiceProvider);
}

app.Run();