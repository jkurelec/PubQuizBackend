using Microsoft.AspNetCore.Authorization;
using PubQuizBackend.Enums;

namespace PubQuizBackend.Auth.RoleAndAudience
{
    public class MinimumRoleAndAudienceRequirement : IAuthorizationRequirement
    {
        public Role MinimumRole { get; }
        public string RequiredAudience { get; }

        public MinimumRoleAndAudienceRequirement(Role minimumRole, string requiredAudience)
        {
            MinimumRole = minimumRole;
            RequiredAudience = requiredAudience;
        }
    }
}
