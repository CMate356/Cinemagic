using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemagic.Models;

namespace Cinemagic.Services
{
    public interface ICinemaService
    {
        Task<IReadOnlyList<Cinema>> SearchAsync(string? query);
        Task<Cinema?> GetByIdAsync(int id);
        Task<List<Cinema>> GetAllCinemasAsync();
    }
}