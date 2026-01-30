using System.Reflection;
using CarX.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarX.Infrastructure.Data;

// ВАЖНО: Наследуемся от IdentityDbContext<User, IdentityRole<long>, long>
public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<long>, long>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    // Твои таблицы (оставил как было)
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Brand> Brands => Set<Brand>();
    // Users можно убрать, так как он есть в IdentityDbContext, но если оставишь - не страшно
    // public DbSet<User> Users => Set<User>(); 
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Rent> Rents => Set<Rent>();
    public DbSet<CarImage> CarImages => Set<CarImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ЭТО ОБЯЗАТЕЛЬНО ДЛЯ IDENTITY (создает таблицы AspNetUsers и т.д.)
        base.OnModelCreating(modelBuilder);

        // Настройка цен
        modelBuilder.Entity<Order>().Property(o => o.FinalPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Rent>().Property(r => r.TotalPrice).HasColumnType("decimal(18,2)");
    
        modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>();
        modelBuilder.Entity<Rent>().Property(r => r.Status).HasConversion<string>();
        
        // Автоматические конфиги
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Твои ручные настройки
        modelBuilder.Entity<Brand>()
            .HasMany(b => b.Cars)
            .WithOne(c => c.Brand)
            .HasForeignKey(c => c.BrandId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Car>()
            .Property(c => c.Class)
            .HasConversion<string>();
    }
}