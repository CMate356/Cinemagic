using System.Collections.Generic;
using Cinemagic.Models;

namespace Cinemagic.ViewModels
{
    public class CinemaSelectionViewModel
    {
        public List<Cinema> Cinemas { get; set; } = new();
        public int? SelectedCinemaId { get; set; }
        public string Query { get; set; }
        public string ReturnUrl { get; set; }
    }
}