using System.ComponentModel.DataAnnotations;

namespace Cinemagic.Models
{
    public class Cinema
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        // Relație: un cinematograf poate avea mai multe proiecții (screenings)
        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}