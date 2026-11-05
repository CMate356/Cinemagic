using Cinemagic.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinemagic.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acțiune dedicată pentru Snacks 
        // Acțiune dedicată pentru Snacks 
        public async Task<IActionResult> Snacks(List<string> categories, decimal? minPrice, decimal? maxPrice, bool inStockOnly)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categories != null && categories.Any())
            {
                query = query.Where(p => categories.Contains(p.Category.Name));
            }
            else
            {
                // Comportament implicit pentru Snacks dacă nu este nimic bifat
                query = query.Where(p => p.Category.Name == "Snacks & Gift Cards" || p.Category.Name == "Snacks");
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (inStockOnly)
            {
                query = query.Where(p => p.StockQuantity > 0);
            }

            var products = await query.ToListAsync();

            return View(products);
        }

        // Acțiune dedicată pentru Merch
        public async Task<IActionResult> Merch(List<string> categories, decimal? minPrice, decimal? maxPrice, bool inStockOnly)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categories != null && categories.Any())
            {
                query = query.Where(p => categories.Contains(p.Category.Name));
            }
            else
            {
                // Implicit afișează Merch dacă nu este nimic bifat
                query = query.Where(p => p.Category.Name == "Merch");
            }

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (inStockOnly)
                query = query.Where(p => p.StockQuantity > 0);

            return View(await query.ToListAsync());
        }
    }
}