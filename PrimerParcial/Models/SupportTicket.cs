using System;
using System.ComponentModel.DataAnnotations;

namespace PrimerParcial.Models
{
    public class SupportTicket
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(255)]
        public string RequesterEmail { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required, MaxLength(50)]
        public string Severity { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [Required]
        public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ClosedAt { get; set; }

        [MaxLength(100)]
        public string? AssignedTo { get; set; }
    }
}