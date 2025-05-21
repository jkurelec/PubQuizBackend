using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PubQuizBackend.Enums;

namespace PubQuizBackend.Auth.RoleAndAudienceFilter
{
    [AttributeUsage(AttributeTargets.All)]
    public class RoleAndAudienceAttribute : Attribute, IAuthorizationFilter
    {
        private readonly Role _minRole;
        private readonly Audience _requiredAudience;

        public RoleAndAudienceAttribute(Role minRole, Audience requiredAudience)
        {
            _minRole = minRole;
            _requiredAudience = requiredAudience;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var role = user.FindFirst(ClaimTypes.Role)?.Value;

            if (!Enum.TryParse(role!.ToUpper(), out Role userRole) || userRole < _minRole)
            {
                context.Result = new ForbidResult();
                return;
            }

            var audience = user.FindFirst("aud")?.Value;

            if (!Enum.TryParse(audience!.ToUpper(), out Audience userAudience)  || userAudience != _requiredAudience)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
