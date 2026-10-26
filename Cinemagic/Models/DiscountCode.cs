using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinemagic.Models
{
    public class DiscountCode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty; // Fixed sau Percentage

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        public DateTime ExpiresAt { get; set; }
        public int MaxUses { get; set; }
        public int UsedCount { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}