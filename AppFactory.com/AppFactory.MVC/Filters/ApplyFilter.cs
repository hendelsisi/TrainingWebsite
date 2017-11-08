using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppFactory.MVC.Repositories;
using AppFactory.DAL;
using AppFactory.MVC.ViewModels;
using System.Web.Routing;

namespace AppFactory.MVC.Filters
{
    public class ApplyFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            GenericUnitOfWork unitOfWork = (GenericUnitOfWork)filterContext.HttpContext.Session["UNITOFWORK"];
            GenericRepository<Application_Applicant> application = unitOfWork.GetRepoInstance<Application_Applicant>();
            ProfileViewModel proVM = (ProfileViewModel)filterContext.HttpContext.Session["USER"];

            var result = application.GetBy(a => a.ApplicantID == proVM.UserID);

            if (result.Count() != 0)
            {
                long applicationId = result.OrderBy(a => a.SubmissionDate).LastOrDefault().ApplicationID;

                if (applicationId == ((Application)filterContext.HttpContext.Application["CurrentApplication"]).ApplicationID)
                {
                    int status = result.OrderBy(a => a.SubmissionDate).LastOrDefault().Status;
                    int step = result.OrderBy(a => a.SubmissionDate).LastOrDefault().Step;
                    bool flag = false;
                    try
                    {
                        string x = filterContext.RequestContext.RouteData.Values["id"].ToString();
                    }
                    catch (Exception)
                    {
                        flag = true;
                    }
                    if (status != 0 && step == 6)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary{
                            { "controller", "User" },
                            { "action", "UserProfile" },
                            { "area", "" }
                            });
                    }
                    else if(flag == false && status != 0)
                    {
                        string action = "ApplicationFormPersonalDetails";
                        if (flag == false)
                        {
                            if (step == 2)
                            {
                                action = "ApplicationFormEducation";
                            }
                            else if (step == 3)
                            {
                                action = "ApplicationFormWorkExperience";
                            }
                            else if (step == 4)
                            {
                                action = "ApplicationFormSkills";
                            }
                            else if (step == 5)
                            {
                                action = "ApplicationFormLanguages";
                            }
                        }
                       
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary{
                            { "controller", "Applicant" },
                            { "action", action },
                            { "area", "" }
                            });
                    }
                }
            }

            //base.OnActionExecuting(filterContext);
        }
    }
}