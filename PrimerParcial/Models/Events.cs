using System;
using System.ComponentModel.DataAnnotations;

namespace PrimerParcial.Models
{
    public partial class Event
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime StartAt { get; set; }   // usa UTC si puedes: DateTime.UtcNow

        public DateTime? EndAt { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}