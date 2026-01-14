using App.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.TimeZone).IsRequired().HasMaxLength(100);
            entity.Property(p => p.IsActive).IsRequired();
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(a => a.StartUtc).IsRequired();
            entity.Property(a => a.EndUtc).IsRequired();
            entity.Property(a => a.Status).IsRequired();
            entity.Property(a => a.CreatedUtc).IsRequired();

            entity.HasOne(a => a.Provider)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.ProviderId, a.StartUtc, a.EndUtc });
        });
    }
}
