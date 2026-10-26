using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinemagic.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }

        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int? ScreeningId { get; set; }
        [ForeignKey("ScreeningId")]
        public Screening? Screening { get; set; }

        public int Quantity { get; set; }
    }
}