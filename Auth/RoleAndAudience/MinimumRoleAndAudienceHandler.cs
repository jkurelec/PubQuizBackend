using Microsoft.AspNetCore.Authorization;
using PubQuizBackend.Enums;
using System.Security.Claims;

namespace PubQuizBackend.Auth.RoleAndAudience
{
    public class MinimumRoleAndAudienceHandler : AuthorizationHandler<MinimumRoleAndAudienceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumRoleAndAudienceRequirement requirement)
        {
            var audienceClaim = context.User.FindFirst("aud")?.Value;
            var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (audienceClaim != requirement.RequiredAudience || roleClaim == null)
                return Task.CompletedTask;

            if (Enum.TryParse<Role>(roleClaim, out var userRole))
            {
                if (userRole >= requirement.MinimumRole)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
