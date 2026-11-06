using Cinemagic.Models;

namespace Cinemagic.ViewModels
{
    public class FilmIndexViewModel
    {
        public Cinema SelectedCinema { get; set; }
        public List<Screening> Screenings { get; set; } = new();
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public string SelectedFormat { get; set; }
        public string SearchQuery { get; set; }
        public List<string> AvailableFormats { get; set; } = new();
    }
}