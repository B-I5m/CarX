using CarX.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CarX.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<User> Users => Set<User>();
// CarX.Infrastructure.Data/ApplicationDbContext.cs
public DbSet<Order> Orders => Set<Order>();
public DbSet<Rent> Rents => Set<Rent>();
// ApplicationDbContext.cs
public DbSet<CarImage> CarImages => Set<CarImage>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    base.OnModelCreating(modelBuilder);

    // Настройка цен, чтобы в БД была нужная точность
    modelBuilder.Entity<Order>().Property(o => o.FinalPrice).HasColumnType("decimal(18,2)");
    modelBuilder.Entity<Rent>().Property(r => r.TotalPrice).HasColumnType("decimal(18,2)");
    
    modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>();
    modelBuilder.Entity<Rent>().Property(r => r.Status).HasConversion<string>();
        // Это важно, чтобы не прописать связи вручную
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Если не хочешь плодить файлы, можно дописать настройки прямо здесь:
        
        // Связь: Один Бренд - Много Машин
        modelBuilder.Entity<Brand>()
            .HasMany(b => b.Cars)
            .WithOne(c => c.Brand)
            .HasForeignKey(c => c.BrandId)
            .OnDelete(DeleteBehavior.Cascade); // Удалил бренд — удалились его машины

        // Настройка Enum для читаемости в БД (сохранять как строку, а не число)
        modelBuilder.Entity<Car>()
            .Property(c => c.Class)
            .HasConversion<string>();

        base.OnModelCreating(modelBuilder);
    }
}