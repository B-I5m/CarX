using CarX.Application.Interfaces;
using CarX.Infrastructure.Services;
using CarX.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CarX.Infrastructure.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// 1. БАЗА ДАННЫХ (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. AUTOMAPPER
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 3. СЕРВИСЫ (Dependency Injection)
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IBrandService, BrandService>();

// Добавь в Program.cs перед builder.Build()
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRentService, RentService>();

// 4. АВТОРИЗАЦИЯ И JWT (Каркас для ролей)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarX API", Version = "v1" });
    
    // Добавляем поддержку JWT в Swagger (чтобы вешать замочек для авторизации)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен"
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ЭТО НУЖНО ДЛЯ КАРТИНКЕ (чтобы по ссылке открывались)
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthentication(); // Кто ты?
app.UseAuthorization();  // Что тебе можно?

app.MapControllers();

// 7. АВТО-МИГРАЦИИ (Удобно для разработки)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();