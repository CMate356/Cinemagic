using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinemagic.Models
{
    public class Screening
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required]
        public int CinemaId { get; set; }
        [ForeignKey("CinemaId")]
        public Cinema? Cinema { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [StringLength(50)]
        public string Format { get; set; } = string.Empty; // ex: VIP 2D, 2D Dubbed

        [Required]
        [StringLength(50)]
        public string Language { get; set; } = string.Empty; // ex: Original, Subtitled

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
    }
}