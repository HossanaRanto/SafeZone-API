using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeZone.DTO;
using SafeZone.Hubs;
using SafeZone.Repositories;

namespace SafeZone.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _user;
        //private readonly ChatHub chat;

        public UserController(IUserRepository user)
        {
            this._user = user;
            //this.chat = chat;
        }

        

        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody] LoginDto login)
        {
            var result = await _user.GetToken(login);
            return result.Match<IActionResult>(
                _ => BadRequest(new
                {
                    Error = _
                }),
                token => Ok(token));
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] UserDto user)
        {
            var result=await _user.Register(user);

            return result.Match<IActionResult>(
                _=>BadRequest(_),
                u=>Ok(u));
        }
        [HttpGet("request_code")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestCode([FromRoute] int userid)
        {
            var user = await _user.Get(userid);
            await _user.GenerateCode(user);

            return Ok(new
            {
                message = "A code has been sent to your account"
            });
        }

        [HttpPost("verify_code")]
        [AllowAnonymous]
        public async Task<IActionResult> Verify([FromBody] VerificationCodeDto code)
        {
            var verify = await _user.VerifyCode(code.Code);
            return verify ? 
                Ok(new { Message = "Your account has been verified" }) 
                : 
                BadRequest(new { Error = "Code invalid. Please retry" });
        }

        [HttpPost("request_reset/{username:string}")]
        public async Task<IActionResult> RequestReset([FromRoute] string username)
        {
            var user = await _user.GetFromUsername(username);
            var result = await _user.RequestPasswordReset(user);

            return Ok(new
            {
                Message = result
            });
        }

        [HttpPost("reset_password")]
        public async Task<IActionResult> Reset([FromBody] ResetPasswordDTO resetPassword)
        {
            var result = await _user.ResetPassword(resetPassword);
            if (!result.Item1)
            {
                return BadRequest(new
                {
                    Error = result.Item2
                });
            }
            return Ok(new
            {
                message = result.Item2
            });
        }
    }
}
