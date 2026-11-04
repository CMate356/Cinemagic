using Cinemagic.Data;
using Cinemagic.Models;
using Cinemagic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinemagic.Controllers
{
    /// Saved delivery addresses of the logged-in user. Every query is filtered by the user's ID.
    /// The database allows only one default address per user (UX_Addresses_OneDefaultPerUser),
    /// so the old default is always cleared before a new one is set.
    [Authorize]
    public class AddressesController(ApplicationDbContext db, CurrentUserService currentUser) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (await currentUser.GetUserIdAsync(User) is not int userId) return Challenge();

            var list = await db.Addresses.AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault).ThenBy(a => a.Id)
                .ToListAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new AddressInputViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddressInputViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (await currentUser.GetUserIdAsync(User) is not int userId) return Challenge();

            var hasAny = await db.Addresses.AnyAsync(a => a.UserId == userId);
            var makeDefault = model.MakeDefault || !hasAny; // the first address is always the default

            if (makeDefault) await ClearDefaultAsync(userId);

            db.Addresses.Add(new Address
            {
                UserId = userId,
                Street = model.Street.Trim(),
                City = model.City.Trim(),
                Phone = model.Phone.Trim(),
                IsDefault = makeDefault
            });
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            if (await currentUser.GetUserIdAsync(User) is not int userId) return Challenge();
            if (!await db.Addresses.AnyAsync(a => a.Id == id && a.UserId == userId)) return NotFound();

            await ClearDefaultAsync(userId);
            await db.Addresses
                .Where(a => a.Id == id && a.UserId == userId)
                .ExecuteUpdateAsync(set => set.SetProperty(a => a.IsDefault, true));

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (await currentUser.GetUserIdAsync(User) is not int userId) return Challenge();

            var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (address is null) return NotFound();

            var wasDefault = address.IsDefault;
            db.Addresses.Remove(address);
            await db.SaveChangesAsync();

            if (wasDefault)
            {
                var next = await db.Addresses.Where(a => a.UserId == userId).OrderBy(a => a.Id).FirstOrDefaultAsync();
                if (next is not null)
                {
                    next.IsDefault = true;
                    await db.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private Task ClearDefaultAsync(int userId) =>
            db.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ExecuteUpdateAsync(set => set.SetProperty(a => a.IsDefault, false));
    }
}