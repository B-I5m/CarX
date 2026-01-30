using CarX.Application.Interfaces;
using CarX.Infrastructure.Services;
using CarX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CarX.Infrastructure.AutoMapper;
using CarX.Domain.Entities; // Добавил
using Microsoft.AspNetCore.Identity; // Добавил

var builder = WebApplication.CreateBuilder(args);

// 1. БАЗА ДАННЫХ (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- НОВОЕ: Настройка Identity для User на long ---
builder.Services.AddIdentity<User, IdentityRole<long>>(config =>
    {
        config.Password.RequiredLength = 4;
        config.Password.RequireDigit = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        config.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// --------------------------------------------------

// 2. AUTOMAPPER
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 3. СЕРВИСЫ (Dependency Injection)
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRentService, RentService>();

// --- НОВОЕ: Регистрация сервиса аккаунтов ---
builder.Services.AddScoped<IAccountService, AccountService>();
// --------------------------------------------

// 4. АВТОРИЗАЦИЯ И JWT
builder.Services.AddAuthentication(options => 
{
    // Обновил настройки, чтобы Identity и JWT дружили
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "SuperSecretKey123456789"))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// 5. КОНТРОЛЛЕРЫ И SWAGGER
builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarX API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен (без слова Bearer, Swagger сам его добавит)"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 6. MIDDLEWARE (ПОРЯДОК ВАЖЕН!)

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

// 7. АВТО-МИГРАЦИИ
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Identity создаст свои таблицы здесь
    await dbContext.Database.MigrateAsync();
}

app.Run();