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
        public string AgeRating { get; set; } = string.Empty;
        public string OriginalTitle { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;

        // Genurile, actorii si regizorii sunt i ntabele separate
        public ICollection<FilmGenre> FilmGenres { get; set; } = new List<FilmGenre>();
        public ICollection<FilmCast> CastMembers { get; set; } = new List<FilmCast>();
        public ICollection<FilmDirector> Directors { get; set; } = new List<FilmDirector>();
    }
}