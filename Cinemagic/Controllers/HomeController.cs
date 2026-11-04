using Cinemagic.Infrastructure;
using Cinemagic.Services;
using Cinemagic.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Cinemagic.Controllers;

public class HomeController : Controller
{
    private readonly ICinemaService _cinemas;

    public HomeController(ICinemaService cinemas) => _cinemas = cinemas;

    // GET /  or  /?q=cluj
    [HttpGet]
    public async Task<IActionResult> Index(string? q)
    {
        var model = new CinemaSelectionViewModel
        {
            Query = q,
            SelectedCinemaId = SelectedCinema.Get(Request),
            Cinemas = (await _cinemas.SearchAsync(q)).ToList()
        };

        return View(model);
    }
}