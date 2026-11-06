using Cinemagic.Data;
using Cinemagic.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinemagic.Services;

// Reads the cinemas from your populated database.
// Assumes your context is ApplicationDbContext and has a DbSet<Cinema> named Cinemas.
public class CinemaService : ICinemaService
{
    private readonly ApplicationDbContext _db;

    public CinemaService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<Cinema>> SearchAsync(string? query)
    {
        var q = query?.Trim();
        IQueryable<Cinema> cinemas = _db.Cinemas.AsNoTracking();

        if (!string.IsNullOrEmpty(q))
        {
            // LIKE in SQLite ignores upper/lower case for normal letters.
            var pattern = $"%{q}%";
            cinemas = cinemas.Where(c =>
                EF.Functions.Like(c.Name, pattern) ||
                EF.Functions.Like(c.City, pattern));
        }

        return await cinemas.OrderBy(c => c.City).ThenBy(c => c.Name).ToListAsync();
    }

    public Task<Cinema?> GetByIdAsync(int id) =>
        _db.Cinemas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Cinema>> GetAllCinemasAsync()
    {
        var list = await _db.Cinemas.AsNoTracking().OrderBy(c => c.City).ThenBy(c => c.Name).ToListAsync();
        return list;
    }
}

