using System.ComponentModel.DataAnnotations;

namespace Cinemagic.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<FilmGenre> FilmGenres { get; set; } = new List<FilmGenre>();
    }
}