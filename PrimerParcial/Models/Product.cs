using System.ComponentModel.DataAnnotations;

namespace PrimerParcial.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}