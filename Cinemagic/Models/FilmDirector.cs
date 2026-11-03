namespace Cinemagic.Models
{
    // Tabel de legatura many-to-many: Film <-> Regizor
    public class FilmDirector
    {
        public int ProductId { get; set; }
        public FilmDetails? FilmDetails { get; set; }

        public int PersonId { get; set; }
        public Person? Person { get; set; }
    }
}