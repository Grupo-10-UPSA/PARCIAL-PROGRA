using Microsoft.EntityFrameworkCore;
using PrimerParcial.Models;

namespace PrimerParcial.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
//test 
        public DbSet<SupportTicket> SupportTickets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SupportTicket>(e =>
            {
                e.Property(p => p.Subject)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(p => p.RequesterEmail)
                    .HasMaxLength(255)
                    .IsRequired();

                // Guarda enums como string para legibilidad (opcional)
                e.Property(p => p.Severity).HasConversion<string>();
                e.Property(p => p.Status).HasConversion<string>();

                // Índices útiles para filtros
                e.HasIndex(p => p.Status);
                e.HasIndex(p => p.Severity);
                e.HasIndex(p => p.OpenedAt);
            });
        }
    }
}