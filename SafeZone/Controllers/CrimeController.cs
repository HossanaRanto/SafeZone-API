using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeZone.DTO;
using SafeZone.Helpers;
using SafeZone.Repositories;

namespace SafeZone.Controllers
{
    [Route("api/crime")]
    [ApiController]
    public class CrimeController : ControllerBase
    {
        private ICrimeRepository _crime;

        public CrimeController(ICrimeRepository crime)
        {
            _crime = crime;
        }

        [HttpPost]
        [Authorize]
        [AuthorizationFilter]
        public async Task<IActionResult> Post([FromBody] PostCrimeDTO crime)
        {
            await _crime.PostCrime(crime);
            return Ok(new
            {
                Message = "Crime posted"
            });
        }

        [HttpGet("list/unclosed")]
        [Authorize]
        [AuthorizationFilter]
        public async Task<IActionResult> GetUnclose(
            [FromQuery(Name ="start")] int? start, 
            [FromQuery(Name ="count")]  int count)
        {
            return Ok(await _crime.GetCrimeForOfficer(start, count));
        }

        [HttpPost("{id:int}/checkout")]
        [Authorize]
        [AuthorizationFilter]
        public async Task<IActionResult> Pay([FromRoute] int id)
        {
            var crime = await _crime.Get(id);
            var pay = await _crime.PassOfficerValidity(crime);
            if (pay.Item1)
            {
                return BadRequest(new
                {
                    Error = pay.Item2
                });
            }
            return Ok(new
            {
                Message = pay.Item2
            });
        }
    }
}
