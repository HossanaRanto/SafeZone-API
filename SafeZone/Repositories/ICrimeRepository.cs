using SafeZone.DTO;

namespace SafeZone.Repositories
{
    public interface ICrimeRepository
    {
        Task PostCrime(PostCrimeDTO postCrime);
    }
}
