using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SafeZone.Repositories;
using System.Security.Claims;

namespace SafeZone.Helpers
{
    public class AuthorizationFilter : Attribute, IAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity != null)
            {
                var _user = context.HttpContext.RequestServices.GetService<IUserRepository>();
                var user_id = context.HttpContext
                    .User.Claims
                    .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value!;

                _user.ConnectedUser = await _user?.Get(int.Parse(user_id))!;
                if (_user.ConnectedUser.IsOfficer)
                {
                    var _officer = context.HttpContext.RequestServices.GetService<IOfficerRepository>();
                    _user.ConnectedOfficer = await _officer.GetFromUser(_user.ConnectedUser.Id);
                }
            }
            context.Result = new UnauthorizedObjectResult("You are not connected");
            return;
        }
    }
}
