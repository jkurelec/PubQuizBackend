using PubQuizBackend.Enums;
using PubQuizBackend.Exceptions;
using System.Security.Claims;

namespace PubQuizBackend.Util.Extension
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var subClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(subClaim, out var userId)
                ? userId
                : throw new UnauthorizedException();
        }

        public static Role GetUserRole(this ClaimsPrincipal user)
        {
            var roleValue = user.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse<Role>(roleValue, ignoreCase: true, out var role)
                ? role
                : throw new UnauthorizedException();
        }
    }

}
