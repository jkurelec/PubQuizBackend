using Microsoft.AspNetCore.Mvc.ApplicationModels;
using PubQuizBackend.Auth.RoleAndAudienceFilter;
using PubQuizBackend.Enums;

namespace PubQuizBackend.Auth.Conventions
{
    public class RoleAndAudienceConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var ns = controller.ControllerType.Namespace?.ToLower() ?? "";

                foreach (var action in controller.Actions)
                {
                    if (ns.Contains(".public"))
                    {
                        action.Filters.Add(new RoleAndAudienceAttribute(
                            minRole: Role.ATTENDEE,
                            requiredAudience: Audience.ATTENDEE
                        ));
                    }
                    else if (ns.Contains(".org"))
                    {
                        action.Filters.Add(new RoleAndAudienceAttribute(
                            minRole: Role.ORGANIZER,
                            requiredAudience: Audience.ORGANIZER
                        ));
                    }
                }
            }
        }
    }
}
