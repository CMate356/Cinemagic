using System.Security.Claims;
using Cinemagic.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinemagic.Services;

/// Finds Users.Id of the logged-in user.
/// AccountController signs in with the e-mail as ClaimTypes.Name and no NameIdentifier claim,
/// so the id is looked up by e-mail. If a NameIdentifier claim with the id is added to the login
/// later, it is used first (no database query).
public class CurrentUserService(ApplicationDbContext db)
{
    public async Task<int?> GetUserIdAsync(ClaimsPrincipal principal, CancellationToken ct = default)
    {
        if (int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            return id;

        var email = principal.Identity?.Name;
        if (string.IsNullOrWhiteSpace(email)) return null;

        return await db.Users.AsNoTracking()
            .Where(u => u.Email == email)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(ct);
    }
}