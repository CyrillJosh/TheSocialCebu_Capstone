using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Menu.Attributes
{
    public class AuthAttribute : ActionFilterAttribute
    {
        private string[] Role;
        public AuthAttribute(string role)
        { 
            Role = role.Split(',');
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            //Get Session
            var user = httpContext.Session.GetString("_Id");
            var role = httpContext.Session.GetString("_Role");
            if (string.IsNullOrEmpty(user) || role.Length == 0 || role == null || !Role.Contains(role))
                context.Result = new RedirectToActionResult("Login", "User", null);
        }
    }
}
