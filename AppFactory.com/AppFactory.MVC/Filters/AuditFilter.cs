using System.Web;
using System.Web.Mvc;
using AppFactory.DAL;
using System.Web.Routing;
using System;
using AppFactory.MVC.ViewModels;
using AppFactory.MVC.Repositories;

namespace AppFactory.MVC.Filters
{
    public class AuditFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            GenericUnitOfWork unitOfWork;
            //Stores the Request in an Accessible object
            var request = filterContext.HttpContext.Request;
            ProfileViewModel user = (ProfileViewModel)filterContext.HttpContext.Session["USER"];

            //Generate an audit from data base
            if (filterContext.HttpContext.Session["USER"] != null)
            {
                //Stores the Audit in the Database
                AuditTrail audit = new AuditTrail()
                {
                    //Our Username (if available)
                    UserName = (request.IsAuthenticated) ?
                                filterContext.HttpContext.User.Identity.Name : "Anonymous",
                    AreaAccessed = request.RawUrl,
                    IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
                    //Creates our Timestamp
                    Time = DateTime.UtcNow
                };
                unitOfWork = (GenericUnitOfWork)filterContext.HttpContext.Session["UNITOFWORK"];
                GenericRepository<AuditTrail> auditRepository = unitOfWork.GetRepoInstance<AuditTrail>();
                auditRepository.Add(audit);
                unitOfWork.SaveChanges();
            }
            //Finishes executing the Action as normal 
            base.OnActionExecuting(filterContext);
        }
    }
}