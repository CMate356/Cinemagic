using Cinemagic.Data;
using Microsoft.AspNetCore.Mvc;

namespace Cinemagic.Controllers
{
    public class CinemasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CinemasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Select(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema != null)
            {
                // Salvăm ID-ul și numele cinema-ului în sesiune
                HttpContext.Session.SetInt32("SelectedCinemaId", cinema.Id);
                HttpContext.Session.SetString("SelectedCinemaName", cinema.Name);
            }

            // Redirecționăm automat către pagina de filme
            return RedirectToAction("Index", "Films");
        }
    }
}