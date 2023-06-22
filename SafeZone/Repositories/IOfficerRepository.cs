using SafeZone.Models;

namespace SafeZone.Repositories
{
    public interface IOfficerRepository
    {
        Task<Officer> Get(int id);
        Task<Officer> GetFromUser(int userid);
    }
}
