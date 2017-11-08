using System.Web.Mvc;
using System.Web.Routing;
using AppFactory.DAL;
using AppFactory.MVC.ViewModels;
using AppFactory.MVC.Repositories;
using System.Linq;
using System.Web;

namespace AppFactory.MVC.Filters
{
    public class AdminFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["USER"] != null)
            {
                GenericUnitOfWork unitOfWork = (GenericUnitOfWork)filterContext.HttpContext.Session["UNITOFWORK"];
                GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
                ProfileViewModel SessionUser = (ProfileViewModel)filterContext.HttpContext.Session["USER"];

                User user = userRepository.GetBy(u => u.UserID == SessionUser.UserID).FirstOrDefault();

                if (user.RoleID != 1)
                {
                    //filterContext.Result = new RedirectToRouteResult(
                    //    new RouteValueDictionary{
                    //        { "controller", "User" },
                    //        { "action", "UserProfile" }, 
                    //        { "area", "" }
                    //    });
                    throw new HttpException(404, "Not found");
                }
            }
            else
            {
                throw new HttpException(404, "Not found");
            }
            
            //base.OnAuthorization(filterContext);
        }
        
    }
}