using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using SafeZone.Data;
using SafeZone.DTO;
using SafeZone.Models;
using SafeZone.Repositories;
using SafeZone.Validators;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SafeZone.Services
{
    public class UserService : IUserRepository
    {
        private readonly DataContext db;
        private readonly IConfiguration configuration;
        private readonly IEmailRepository email;
        static Random random_code = new Random();


        public UserService(DataContext db, IConfiguration configuration, IEmailRepository email)
        {
            this.db = db;
            this.configuration = configuration;
            this.email = email;
        }
        public string HashPassword(string password)
        {
            string key = configuration.GetValue<string>(
                Variables.Key);

            var key_password = Encoding.UTF8.GetBytes(key);
            using (var hmac = new HMACSHA512(key_password))
            {
                var pwd = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(pwd);
                //return Encoding.UTF8.GetString(pwd);
            }
        }
        public async Task<OneOf<string, Token>> GetToken(LoginDto user)
        {
            var u = await db.Users.FirstOrDefaultAsync(u =>
            u.Username == user.Username &&
            u.Password == HashPassword(user.Password));

            if (u != null)
            {
                return await GetToken(u);
            }

            return "Account not found";
        }

        public async Task<OneOf<string,Token>> GetToken(User user)
        {
            if (!user.IsVerfied)
            {
                return "Your account is not verified";
            }
            if (user.IsBlocked)
            {
                return "Your account is blocked";
            }
            return new Token
            {
                AccessToken = GenerateToken(user, DateTime.Now.AddHours(2)),
                RefreshToken = GenerateToken(user, DateTime.Now.AddDays(1))
            };
        }

        public string GenerateToken(User user,DateTime expiredate)
        {
            string token_key = configuration.GetValue<string>(
                Variables.Token);

            byte[] key = System.Text.Encoding.UTF8.GetBytes(token_key);
            var symmetric_key = new SymmetricSecurityKey(key);
            var credential = new SigningCredentials(symmetric_key, SecurityAlgorithms.HmacSha256Signature);
            var header = new JwtHeader(credential);
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };
            //var paylod = new JwtPayload(id.ToString(), audience: null, claims: claims, notBefore: null, expires: DateTime.Today.AddDays(1));
            var securitytoken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credential
                );
            return new JwtSecurityTokenHandler()
                .WriteToken(securitytoken);
        }
        public Task RefreshToken()
        {
            throw new NotImplementedException();
        }

        public async Task<User> Get(int userid)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userid);
            return user;
        }

        public async Task<User> GetFromUsername(string username)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        public async Task<OneOf<ValidationFailed, User>> Register(UserDto userdto)
        {
            var validator = new UserValidator();
            var validator_result = validator.Validate(userdto);

            if (!validator_result.IsValid)
            {
                return new ValidationFailed(validator_result.Errors);
            }

            var user = new User
            {
                Username = userdto.Username,
                Password = HashPassword(userdto.Password),
                Email = userdto.Email,
                Phone = userdto.Phone,
                Birthdate = userdto.Birthdate,
                Gender = userdto.Gender,
                Coordinates = userdto.Coordinates,
                Firstname = userdto.Firstname,
                Lastname = userdto.Lastname
            };

            db.Users.Add(user);
            db.SaveChanges();

            await this.GenerateCode(user);
            

            return user;
        }

        public async Task<string> GenerateCode(User user)
        {
            var code_int = random_code.Next(0, 9999);
            var unique_code = $"{code_int}-{user.Id}";
            var hashed_code = System.Text.Encoding.UTF8.GetBytes(unique_code);

            var code = await db.VerificationCodes
                .Include(c=>c.User)
                .FirstOrDefaultAsync(c => c.User == user);

            if (code == null)
            {
                code = new VerificationCode { User = user };
                db.VerificationCodes.Add(code);
            }

            code.Code=Convert.ToBase64String(hashed_code);

            await db.SaveChangesAsync();

            //email.Send(
            //    user.Email,
            //    "Safe Zone - Verification Code",
            //    $"This is your code to verify your account:\nCode: {code.Code}");
            return code.Code;
        }

        public async Task<bool> VerifyCode(string code)
        {
            var code_obj = await db.VerificationCodes
                .Include(c=>c.User)
                .FirstOrDefaultAsync(c => c.Code == code);
            if(code_obj == null)
            {
                return false;
            }
            return await VerifyCode(code_obj);
        }

        public async Task<bool> VerifyCode(VerificationCode code)
        {
            code.User.IsVerfied= true;
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<string> RequestPasswordReset(User user)
        {
            var code = random_code.Next(100000, 999999);

            var reset = new PasswordResetCode
            {
                User = user,
                Code = code
            };
            await email.Send(
                user.Email,
                "Code de réinitialisation du mot de passe",
                $"Voici votre code de réinitialisation {code}. Veuillez ne pas le partager. " +
                $"Valide pendant 3 heures");

            return "Password code sent";
        }

        public async Task<Tuple<bool,string>> ResetPassword(ResetPasswordDTO resetPassword)
        {
            var user = await this.GetFromUsername(resetPassword.Username);

            var reset = await this.db.PasswordResetCodes.FirstOrDefaultAsync(p =>
            p.User == user &&
            p.Code == resetPassword.Code &&
            p.Validity <= DateTime.Now);

            if (reset == null)
            {
                return new Tuple<bool, string>(false, "Code invalid or outdated");
            }

            user.Password= resetPassword.Password;
            db.SaveChanges();

            return new Tuple<bool, string>(true, "Password reset");
        }

        public async Task<List<Tuple<double, User>>> GetUsersNearAPosition(string coordinates)
        {
            var list = Enumerable.Empty<Tuple<double,User>>().ToList();
            var users = await this.db.Users.ToListAsync();
            var original_coordinates = CoordinateCalculation.GetCoordinate(coordinates);
            foreach(var user in users)
            {
                var position = CoordinateCalculation.GetCoordinate(user.Coordinates);
                var distance = CoordinateCalculation.CalculateDistance(position, original_coordinates);

                if (distance <= Variables.Distance)
                {
                    list.Add(new Tuple<double, User>(distance,user));
                }
            }

            return list;
        }
    }
}
