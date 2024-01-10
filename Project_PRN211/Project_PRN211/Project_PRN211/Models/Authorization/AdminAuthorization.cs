using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Project_PRN211.Models.Authorization
{
    public class AdminAuthorization :ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if(context.HttpContext.Session.GetString("IsAdmin")!="1")
            {
                context.Result = new RedirectToRouteResult(
                   new RouteValueDictionary
                   {
                        {"Controller", "Access" },
                        {"Action", "Login" }
                   });
            }
        }
    }
}
