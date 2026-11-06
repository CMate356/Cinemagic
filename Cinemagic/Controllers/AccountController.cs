using System.Security.Claims;
using Cinemagic.Data;
using Cinemagic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinemagic.Controllers
{
    public class AccountController(ApplicationDbContext db) : Controller
    {
        // Hash fals, folosit cand emailul nu exista, ca sa nu se poata afla ce emailuri sunt inregistrate
        private static readonly string DummyHash = BCrypt.Net.BCrypt.HashPassword("dummy-password-not-used");

        // Inregistrare
        [HttpGet]
        public IActionResult Register() =>
            User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home") : View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = model.Email.Trim().ToLowerInvariant();
            if (await db.Users.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError(nameof(model.Email), "Acest email este deja folosit.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName.Trim(),
                Email = email,
                Role = "Customer", // rolul se seteaza doar pe server, nu vine din formular
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            db.Users.Add(user);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException) // doua inregistrari simultane cu acelasi email
            {
                ModelState.AddModelError(nameof(model.Email), "Acest email este deja folosit.");
                return View(model);
            }

            await SignInUserAsync(user);
            return RedirectToAction("Index", "Home");
        }

        // Autentificare
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToLocal(returnUrl);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var email = model.Email.Trim().ToLowerInvariant();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Verificam mereu o parola, chiar daca utilizatorul nu exista
            var passwordOk = VerifyPassword(model.Password, user?.PasswordHash ?? DummyHash);
            if (user is null || !passwordOk)
            {
                ModelState.AddModelError(string.Empty, "Email sau parolă incorectă.");
                return View(model);
            }

            await SignInUserAsync(user);
            return RedirectToLocal(returnUrl);
        }

        // Deconectare
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Pagina afisata cand rolul utilizatorului nu are voie la o pagina
        [HttpGet]
        public IActionResult AccessDenied() => View();

        // Metode ajutatoare
        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.GivenName, user.FullName),
                new(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        private static bool VerifyPassword(string password, string hash)
        {
            try { return BCrypt.Net.BCrypt.Verify(password, hash); }
            catch (Exception) { return false; } // hash invalid, de exemplu un hash vechi PBKDF2
        }

        // Redirectionam doar catre adrese din aplicatie (protectie impotriva open redirect)
        private IActionResult RedirectToLocal(string? returnUrl) =>
            !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? LocalRedirect(returnUrl)
                : RedirectToAction("Index", "Home");
    }
}