using Microsoft.AspNetCore.Mvc;

namespace PubQuizBackend.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult OkAtAction(string actionName, object routeValues, object value)
        {
            var url = Url.Action(actionName, routeValues);
            Response.Headers.Location = url;
            return Ok(value);
        }

        protected IActionResult NoContentAtAction(string actionName, object routeValues)
        {
            var url = Url.Action(actionName, routeValues);
            Response.Headers.Location = url;
            return NoContent();
        }
    }
}
