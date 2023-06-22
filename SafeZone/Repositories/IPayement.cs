using SafeZone.Models;

namespace SafeZone.Repositories
{
    public interface IPayement
    {
        Task<string> Create(Crime crimes);
    }
}
