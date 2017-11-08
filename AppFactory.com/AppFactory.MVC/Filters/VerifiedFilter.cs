using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AppFactory.MVC.Repositories;
using AppFactory.DAL;
using AppFactory.MVC.ViewModels;

namespace AppFactory.MVC.Filters
{
    public class VerifiedFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["USER"] != null)
            {
                GenericUnitOfWork unitOfWork = (GenericUnitOfWork)filterContext.HttpContext.Session["UNITOFWORK"];
                GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
                ProfileViewModel SessionUser = (ProfileViewModel)filterContext.HttpContext.Session["USER"];

                User user = userRepository.GetBy(u => u.UserID == SessionUser.UserID).FirstOrDefault();

                if (user.EmailConfirmed != true)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary{
                            { "controller", "User" },
                            { "action", "UserProfile" },
                            { "area", "" }
                        });
                }

            }

            //base.OnActionExecuting(filterContext);
        }
    }
}