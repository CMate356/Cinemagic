using Cinemagic.Data;
using Cinemagic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinemagic.Controllers
{
    public class FilmsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilmsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> SelectCinema(string name, int? id)
        {
            var cinema = await _context.Cinemas
                .FirstOrDefaultAsync(c => (id.HasValue && c.Id == id.Value) || (name != null && c.Name.Contains(name)));

            if (cinema != null)
            {
                HttpContext.Session.SetInt32("SelectedCinemaId", cinema.Id);
                HttpContext.Session.SetString("SelectedCinemaName", cinema.Name);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Index(DateTime? date, string format, string search, int? cinemaId)
        {
            if (!cinemaId.HasValue)
            {
                cinemaId = HttpContext.Session.GetInt32("SelectedCinemaId");
            }

            if (!cinemaId.HasValue)
            {
                return RedirectToAction("Index", "Home", new { returnUrl = Request.Path });
            }

            var cinema = await _context.Cinemas.FindAsync(cinemaId.Value);
            if (cinema == null)
            {
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetInt32("SelectedCinemaId", cinema.Id);
            HttpContext.Session.SetString("SelectedCinemaName", cinema.Name);

            var targetDate = date?.Date ?? DateTime.Today;

            var query = _context.Screenings
                .Include(s => s.Product)
                    .ThenInclude(p => p.FilmDetails)
                        .ThenInclude(fd => fd.FilmGenres)
                            .ThenInclude(fg => fg.Genre)
                .Include(s => s.Cinema)
                .Where(s => s.CinemaId == cinemaId.Value && s.StartTime.Date == targetDate);

            if (!string.IsNullOrEmpty(format))
            {
                query = query.Where(s => s.Format == format);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Product.Title.Contains(search));
            }

            var screeningsList = await query.OrderBy(s => s.StartTime).ToListAsync();

            var formats = await _context.Screenings
                .Where(s => s.CinemaId == cinemaId.Value)
                .Select(s => s.Format)
                .Distinct()
                .ToListAsync();

            var viewModel = new FilmIndexViewModel
            {
                SelectedCinema = cinema,
                Screenings = screeningsList,
                SelectedDate = targetDate,
                SelectedFormat = format,
                SearchQuery = search,
                AvailableFormats = formats
            };

            return View(viewModel);
        }
    }
}