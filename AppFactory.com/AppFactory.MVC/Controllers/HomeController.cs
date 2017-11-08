using AppFactory.DAL;
using AppFactory.MVC.Repositories;
using AppFactory.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppFactory.MVC.Controllers
{
    public class HomeController : Controller
    {
        GenericUnitOfWork unitOfWork;

        public ActionResult Index()
        {
            if (Session["USER"] == null)
            {
                if (Request.IsAuthenticated)
                {
                    unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
                    var claimsPrincipalCurrent = System.Security.Claims.ClaimsPrincipal.Current;
                    string email = claimsPrincipalCurrent.FindFirst("preferred_username").Value;
                    GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
                    User user = userRepository.GetBy(u => u.Email == email).FirstOrDefault();

                    if (user != null)
                    {
                        // Already Registered / Redirect to profile
                        ProfileViewModel proVM = new ProfileViewModel()
                        {
                            UserID = user.UserID,
                            Avatar = user.Avatar,
                            Email = user.Email,
                            PostalCode = user.PersonalDetail.PostalCode,
                            FirstName = user.PersonalDetail.FirstName,
                            LastName = user.PersonalDetail.LastName,
                            CountryID = user.PersonalDetail.CountryID,
                            Country = user.PersonalDetail.Country,
                            Gender = user.PersonalDetail.Gender,
                            MobileNumber = user.PersonalDetail.MobileNumber,
                            DateOfBirth = user.PersonalDetail.DateOfBirth,
                            ExternLogin = true
                        };
                        
                        Session["USER"] = proVM;

                        return RedirectToAction("UserProfile", "User");
                    }
                    else
                    {
                        // Not Registered yet / Redirect to register form
                        RegisterationViewModel regVM = new RegisterationViewModel()
                        {
                            FirstName = claimsPrincipalCurrent.FindFirst("name").Value,
                            Email = claimsPrincipalCurrent.FindFirst("preferred_username").Value,
                            ExternLogin = true
                        };
                        return RedirectToAction("MicrosoftRegister", "User", regVM);
                    }
                }

            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetStarted()
        {
            return View();
        }
    }
}