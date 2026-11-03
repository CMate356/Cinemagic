using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinemagic.Models
{
    // Cheie compusa (UserId, ProductId): o singura recenzie per utilizator si produs
    public class Review
    {
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}