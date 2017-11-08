using System.Web.Mvc;
using System.Web.Routing;

namespace AppFactory.MVC.Filters
{
    public class LoggedInFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["USER"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary{
                    { "controller", "User" },
                    { "action", "Login" },
                    { "area", "" }
                });
            }

            //base.OnAuthorization(filterContext);
        }
        
    }
}