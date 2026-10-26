using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinemagic.Models
{
    public class StripeEvent
    {
        [Key]
        [StringLength(100)]
        public string EventId { get; set; } = string.Empty;

        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        public DateTime ProcessedAt { get; set; } = DateTime.Now;
    }
}