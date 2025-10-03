using Microsoft.EntityFrameworkCore;
using KernelPagBank.Models;

namespace KernelPagBank.Data;

/// <summary>
/// Contexto do banco de dados SQLite
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<Sale> Sales => Set<Sale>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações da entidade Sale
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            
            entity.HasOne(e => e.Seller)
                  .WithMany(s => s.Sales)
                  .HasForeignKey(e => e.SellerId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.SaleDate);
        });

        // Configurações da entidade Seller
        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}

