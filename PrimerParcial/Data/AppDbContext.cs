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

        // DbSets usados por tus 3 controllers
        public DbSet<SupportTicket> SupportTickets { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------- SupportTicket ----------
            modelBuilder.Entity<SupportTicket>(e =>
            {
                e.Property(p => p.Subject)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(p => p.RequesterEmail)
                    .HasMaxLength(255)
                    .IsRequired();

                // Severity/Status son string (no enums)
                e.Property(p => p.Severity)
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(p => p.Status)
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(p => p.AssignedTo)
                    .HasMaxLength(100);

                // Índices para consultas
                e.HasIndex(p => p.Status);
                e.HasIndex(p => p.Severity);
                e.HasIndex(p => p.OpenedAt);
            });

            // ---------- Event ----------
            modelBuilder.Entity<Event>(e =>
            {
                e.Property(p => p.Title)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(p => p.Location)
                    .HasMaxLength(200)
                    .IsRequired();

                e.Property(p => p.StartAt)
                    .IsRequired();

                // EndAt y Notes son opcionales por diseño
                // e.Property(p => p.EndAt); // nullable
                // e.Property(p => p.Notes); // nullable (texto libre)

                // Índices útiles
                e.HasIndex(p => p.StartAt);
                e.HasIndex(p => p.IsOnline);
            });

            // ---------- Product ----------
            // No imponemos restricciones extra porque tu controlador no las requiere.
            // (Solo asegura que exista la entidad Product con su Id como PK convencional)
            modelBuilder.Entity<Product>();
        }
    }
}
