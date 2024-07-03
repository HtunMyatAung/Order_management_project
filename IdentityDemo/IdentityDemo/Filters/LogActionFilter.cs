using IdentityDemo.Data;
using IdentityDemo.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Security.Claims;

namespace IdentityDemo.Filters
{
    public class LogActionFilter:IAsyncActionFilter
    {
        private readonly AppDbContext _context;

        public LogActionFilter(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Before the action executes
            var userName = context.HttpContext.User.Identity.Name;
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim != null ? userIdClaim.Value : null; // Retrieve UserId from claims

            var actionName = context.ActionDescriptor.RouteValues["action"];
            var controllerName = context.ActionDescriptor.RouteValues["controller"];
            var requestData = JsonConvert.SerializeObject(context.ActionArguments);
            var timestamp = DateTime.Now;

            var log = new ActionLog
            {
                UserId=userId,
                LogStatus="Route",
                ActionName = actionName,
                ControllerName = controllerName,
                Timestamp = timestamp,
                RequestData = null
            };

            _context.ActionLogs.Add(log);
            await _context.SaveChangesAsync();

            // After the action executes
            var executedContext = await next();
        }
    }
}
