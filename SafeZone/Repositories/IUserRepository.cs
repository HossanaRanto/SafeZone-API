using OneOf;
using SafeZone.DTO;
using SafeZone.Models;
using SafeZone.Validators;

namespace SafeZone.Repositories
{
    public interface IUserRepository
    {
        string HashPassword(string password);
        string GenerateToken(User user, DateTime expiredate);
        Task<OneOf<string, Token>> GetToken(LoginDto user);
        Task<OneOf<string, Token>> GetToken(User user);
        Task RefreshToken();
        Task<User> Get(int userid);
        Task<User> GetFromUsername(string username);
        Task<OneOf<ValidationFailed,User>> Register(UserDto user);
        Task<string> GenerateCode(User user);
        Task<bool> VerifyCode(string code);
        Task<bool> VerifyCode(VerificationCode code);
        Task<string> RequestPasswordReset(User user);
        Task<Tuple<bool, string>> ResetPassword(ResetPasswordDTO resetPassword);
        Task<List<Tuple<double,User>>> GetUsersNearAPosition(string coordinates);
    }
}
