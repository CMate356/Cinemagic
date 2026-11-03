namespace Cinemagic.Models
{
    // Tabel de legatura many-to-many: Film <-> Gen
    public class FilmGenre
    {
        public int ProductId { get; set; }
        public FilmDetails? FilmDetails { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}