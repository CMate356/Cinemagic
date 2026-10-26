using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinemagic.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public int DeliveryMethodId { get; set; }
        [ForeignKey("DeliveryMethodId")]
        public DeliveryMethod? DeliveryMethod { get; set; }

        public int? DiscountCodeId { get; set; }
        [ForeignKey("DiscountCodeId")]
        public DiscountCode? DiscountCode { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        [StringLength(50)]
        public string Status { get; set; } = "Pending";
        [StringLength(150)]
        public string ContactEmail { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryCost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string StripeSessionId { get; set; } = string.Empty;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<StripeEvent> StripeEvents { get; set; } = new List<StripeEvent>();
    }
}