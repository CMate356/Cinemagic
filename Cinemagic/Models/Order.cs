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

        // Adresa de livrare in coloane atomice (copie la momentul comenzii)
        [Required]
        [StringLength(200)]
        public string ShippingStreet { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string ShippingCity { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string ShippingPhone { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryCost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [StringLength(100)]
        public string? StripeSessionId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<StripeEvent> StripeEvents { get; set; } = new List<StripeEvent>();

        // Valori calculate, nu coloane. Necesita Include(o => o.OrderItems).
        [NotMapped]
        public decimal Subtotal => OrderItems.Sum(i => i.Quantity * i.UnitPrice);

        [NotMapped]
        public decimal TotalAmount => Subtotal + DeliveryCost - DiscountAmount;
    }
}