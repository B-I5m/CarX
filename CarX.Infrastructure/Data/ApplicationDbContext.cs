using System.Reflection;
using CarX.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarX.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<long>, long>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Rent> Rents => Set<Rent>();
    public DbSet<CarImage> CarImages => Set<CarImage>();
    public DbSet<RentCar> RentCars => Set<RentCar>(); // Добавил явный тип
    public DbSet<RentCarImage> RentCarImages => Set<RentCarImage>();
    public DbSet<Favorite> Favorites { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Настройка цен и валют ---
        modelBuilder.Entity<Order>().Property(o => o.FinalPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Rent>().Property(r => r.TotalPrice).HasColumnType("decimal(18,2)");
        
        // НОВОЕ: Настройка для RentCar (чтобы не было warning в консоли Rider)
        modelBuilder.Entity<RentCar>().Property(rc => rc.PricePerDay).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<RentCar>().Property(rc => rc.Deposit).HasColumnType("decimal(18,2)");
    
        // --- Конвертация Enum в строки (чтобы в БД было "Pending", а не 0) ---
        modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>();
        modelBuilder.Entity<Rent>().Property(r => r.Status).HasConversion<string>();
        modelBuilder.Entity<Car>().Property(c => c.Class).HasConversion<string>();
        
        // --- Связи (Relations) ---
        
        // Связь Brand -> Cars (Продажа)
        modelBuilder.Entity<Brand>()
            .HasMany(b => b.Cars)
            .WithOne(c => c.Brand)
            .HasForeignKey(c => c.BrandId)
            .OnDelete(DeleteBehavior.Cascade);

        // НОВОЕ: Связь Brand -> RentCars (Аренда)
        modelBuilder.Entity<RentCar>()
            .HasOne(rc => rc.Brand)
            .WithMany() // Если в Brand нет коллекции RentCars, оставляем пустым
            .HasForeignKey(rc => rc.BrandId)
            .OnDelete(DeleteBehavior.Cascade);

        // Автоматические конфиги из файлов в этой же папке
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}