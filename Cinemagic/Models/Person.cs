using System.ComponentModel.DataAnnotations;

namespace Cinemagic.Models
{
    // O persoană poate fi actor si/sau regizor.
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        public ICollection<FilmCast> CastRoles { get; set; } = new List<FilmCast>();
        public ICollection<FilmDirector> DirectedFilms { get; set; } = new List<FilmDirector>();
    }
}