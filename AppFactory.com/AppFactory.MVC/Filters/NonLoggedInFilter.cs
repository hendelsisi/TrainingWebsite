using System.Web.Mvc;
using System.Web.Routing;

namespace AppFactory.MVC.Filters
{
    public class NonLoggedInFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["USER"] != null)
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary{
                    { "controller", "User" },
                    { "action", "UserProfile" }
                });
            }

            //base.OnActionExecuting(filterContext);
        }
    }
}