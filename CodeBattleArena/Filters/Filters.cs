using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CodeBattleArena.Filters
{
    public class CheckSessionCookieAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var hasSessionCookie = context.HttpContext.Request.Cookies.ContainsKey("IdSession");

            if (!hasSessionCookie)
            {
                context.Result = new RedirectToActionResult("HomePage", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}