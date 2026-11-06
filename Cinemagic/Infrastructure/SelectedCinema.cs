using Microsoft.AspNetCore.Http;

namespace Cinemagic.Infrastructure
{
    public static class SelectedCinema
    {
        private const string CookieName = "SelectedCinemaId";

        public static int? Get(HttpRequest request)
        {
            if (request.Cookies.TryGetValue(CookieName, out var val) && int.TryParse(val, out var id))
            {
                return id;
            }
            return null;
        }

        public static void Set(HttpResponse response, int cinemaId)
        {
            response.Cookies.Append(CookieName, cinemaId.ToString(), new CookieOptions
            {
                Expires = System.DateTimeOffset.UtcNow.AddDays(30),
                IsEssential = true
            });
        }
    }
}