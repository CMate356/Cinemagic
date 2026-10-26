using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinemagic.Models
{
    public class FilmDetails
    {
        [Key, ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int DurationMinutes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Agerating { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Cast { get; set; } = string.Empty;
        public string OriginalTitle { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
    }
}