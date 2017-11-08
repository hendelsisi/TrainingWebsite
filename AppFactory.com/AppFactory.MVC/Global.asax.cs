using AppFactory.DAL;
using AppFactory.MVC.Repositories;
using AppFactory.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IdentityModel.Claims;

namespace AppFactory.MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Application["noofVisitors"] = 0;
            Application["LoggedInUsers"] = 0;
            Application["CurrentApplication"] = null;

            System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["UNITOFWORK"] = new GenericUnitOfWork();

            Application.Lock();
            Application["noofVisitors"] = (int)Application["noofVisitors"] + 1;
            Application.UnLock();

            GenericUnitOfWork unitOfWork;
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            if (Application["CurrentApplication"] == null)
            {
                GenericRepository<Application> appRepository = unitOfWork.GetRepoInstance<Application>();
                Application application = appRepository.GetAll().LastOrDefault();

                if (application.EndDate > DateTime.Now)
                {
                    Application["CurrentApplication"] = application;
                }
            }

            if (Request.Cookies["LoginCookie"] != null)
            {
                long id;
                string token = Request.Cookies["LoginCookie"]["RemeberMe"];
                Int64.TryParse(Request.Cookies["LoginCookie"]["UserID"].ToString(), out id);

                unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

                GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
                User testedUser = userRepository.GetBy(u => u.UserID == id).FirstOrDefault();

                if (testedUser != null)
                {
                    if (testedUser.RememberToken == token)
                    {
                        ProfileViewModel proVM = new ProfileViewModel()
                        {
                            UserID = testedUser.UserID,
                            Avatar = testedUser.Avatar,
                            Email = testedUser.Email,
                            PostalCode = testedUser.PersonalDetail.PostalCode,
                            FirstName = testedUser.PersonalDetail.FirstName,
                            LastName = testedUser.PersonalDetail.LastName,
                            CountryID = testedUser.PersonalDetail.CountryID,
                            Country = testedUser.PersonalDetail.Country,
                            Gender = testedUser.PersonalDetail.Gender,
                            MobileNumber = testedUser.PersonalDetail.MobileNumber,
                            DateOfBirth = testedUser.PersonalDetail.DateOfBirth
                        };
                        
                        Session["USER"] = proVM;
                    }
                }
            }
            else
            {
            }
        }

        protected void Session_End(Object sender, EventArgs e)
        {
            ProfileViewModel sessionUser = (ProfileViewModel)Session["USER"];

            if (sessionUser != null)
            {
                // Make that user offline.
                GenericUnitOfWork unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
                GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();

                User user = userRepository
                    .GetBy(u => u.UserID == sessionUser.UserID)
                    .FirstOrDefault();
                user.Online = false;
                userRepository.Edit(user);
                unitOfWork.SaveChanges();
            }

            Application.Lock();
            Application["noofVisitors"] = (int)Application["noofVisitors"] - 1;
            Application.UnLock();
        }

        protected void Application_End()
        {

        }
    }
}
