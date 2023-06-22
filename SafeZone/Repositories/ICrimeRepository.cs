using SafeZone.DTO;
using SafeZone.Models;

namespace SafeZone.Repositories
{
    public interface ICrimeRepository
    {
        Task<Crime> Get(int id);
        Task PostCrime(PostCrimeDTO postCrime);
        Task<List<Crime>> GetCrimeForOfficer(int? start,int? count);
        Task CloseCase(Crime crime);
        Task<Tuple<bool, string>> PassOfficerValidity(Crime crime);
    }
}
