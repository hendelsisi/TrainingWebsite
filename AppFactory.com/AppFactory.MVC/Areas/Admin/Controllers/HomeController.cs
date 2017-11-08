using AppFactory.DAL;
using AppFactory.Mailing;
using AppFactory.MVC.Repositories;
using AppFactory.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppFactory.MVC.Filters;

namespace AppFactory.MVC.Areas.Admin.Controllers
{
    [AdminFilter]
    public class HomeController : Controller
    {
        GenericUnitOfWork unitOfWork;

        // GET: Admin/Home
        public ActionResult Dashboard()
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            GenericRepository<Applicant> appRepository = unitOfWork.GetRepoInstance<Applicant>();

            int NonLoggedIn = (int)System.Web.HttpContext.Current.Application["noofVisitors"] - (int)System.Web.HttpContext.Current.Application["LoggedInUsers"];

            ViewBag.LoggedInUsers = (int)System.Web.HttpContext.Current.Application["LoggedInUsers"];


            ViewBag.AllUsers = userRepository.GetAll().Count();
            ViewBag.NonLoggedIn = NonLoggedIn;
            ViewBag.PendingAppNum = appRepository.GetAll().Count();
            ViewBag.percent = (ViewBag.online / ViewBag.subcount) * 100;
            if (Session["USER"] != null)
            {
                ProfileViewModel admin = (ProfileViewModel)Session["USER"];
                ViewBag.User = admin.FirstName + " " + admin.LastName;
            }
            else
            {
                ViewBag.User = "ADMIN";
            }


            return View();
        }

        [HttpPost]
        public string OpenApp(Application app)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<Application> appRepository = unitOfWork.GetRepoInstance<Application>();

            if ((app.StartDate < DateTime.Now) || (app.StartDate > app.EndDate))
            {
                return "Please Enter Valid Dates";
            }
            var result = appRepository.GetAll();
            if (result != null)
            {
                Application lastApp = result.LastOrDefault();
                if (app.StartDate <= lastApp.EndDate)
                {
                    return "You can't add application with start date before " + (lastApp.EndDate.AddDays(1));
                }
            }

            appRepository.Add(app);
            unitOfWork.SaveChanges();
            ViewBag.Error = "Error Bag";

            return "Done";
        }

        public ActionResult SendMessage(Message message)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<Message> messageRepository = unitOfWork.GetRepoInstance<Message>();

            message.SentDate = DateTime.Now;

            messageRepository.Add(message);
            unitOfWork.SaveChanges();
            return RedirectToAction("AppsList");
        }

        //Users List
        public ActionResult UsersList()
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();

            List<User> Users = new List<User>();
            var result = userRepository.GetAll();

            foreach (User user in result)
            {
                Users.Add(user);
            }

            if (Session["USER"] != null)
            {
                ProfileViewModel admin = (ProfileViewModel)Session["USER"];
                ViewBag.User = admin.FirstName + " " + admin.LastName;
            }
            else
            {
                ViewBag.User = "Amira Mahmoud";
            }
            return View(Users);
        }

        public ActionResult ResetPasswd(int id)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            GenericRepository<PersonalDetail> detailsRepository = unitOfWork.GetRepoInstance<PersonalDetail>();

            User user = userRepository.GetBy(u => u.UserID == id).FirstOrDefault();
            PersonalDetail userDetails = detailsRepository.GetBy(p => p.User.UserID == id).FirstOrDefault();
            //Send confirmation email
            try
            {
                if (ModelState.IsValid)
                {

                    string body = string.Format("Dear {0} , <br/> Please Change Your Password, please click on the below link to complete reseting your password:<a href=\"{1}\"title=\"User Email Confirm\">{1}</a>",
                        userDetails.FirstName + " " + userDetails.LastName,
                        Url.Action("ResetPassword", "User", new { Token = user.VerificationCode, Email = user.Email, area = "" }, Request.Url.Scheme));

                    Mailing.EmailConfig conf = new Mailing.EmailConfig("amirademo89@gmail.com", "AppFactory.com demo", "demowebsite")
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true
                    };

                    Email email = new Email(conf);
                    email.Send(user.Email, "Reset Your Password", body);
                }
            }
            catch (Exception)
            {

            }

            return RedirectToAction("UsersList");
        }

        public ActionResult ChangeRole(User user)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();

            User updatedUser = userRepository.GetBy(u => u.UserID == user.UserID).FirstOrDefault();
            if (updatedUser != null)
            {
                if (updatedUser.RoleID == 2)
                {
                    updatedUser.RoleID = 1;
                }
                else if (updatedUser.RoleID == 1)
                {
                    updatedUser.RoleID = 2;
                }

                userRepository.Edit(updatedUser);
                unitOfWork.SaveChanges();
            }

            return RedirectToAction("UsersList");
        }

        //Apps List
        public ActionResult AppsList()
        {
            ProfileViewModel admin = (ProfileViewModel)Session["USER"];
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            GenericRepository<Applicant> applicantRepository = unitOfWork.GetRepoInstance<Applicant>();
            GenericRepository<Application_Applicant> applicationRepository = unitOfWork.GetRepoInstance<Application_Applicant>();
            GenericRepository<PersonalDetail> detailsRepository = unitOfWork.GetRepoInstance<PersonalDetail>();

            var result = applicantRepository.GetAll();

            List<ApplicantListViewModel> appsList = new List<ApplicantListViewModel>();
            PersonalDetail details;
            Application_Applicant app;

            foreach (Applicant a in result)
            {
                ApplicantListViewModel model = new ApplicantListViewModel();
                app = applicationRepository.GetBy(p => p.ApplicantID == a.ApplicantID).LastOrDefault();
                details = detailsRepository.GetBy(p => p.User.UserID == a.User.UserID).FirstOrDefault();
                model.AdminID = admin.UserID;
                model.FirstName = details.FirstName;
                model.LastName = details.LastName;
                model.ApplicantID = a.ApplicantID;
                model.Email = a.User.Email;
                model.Status = app.Status;
                appsList.Add(model);

            }

            if (Session["USER"] != null)
            {
                ProfileViewModel Admin = (ProfileViewModel)Session["USER"];
                ViewBag.User = Admin.FirstName + " " + Admin.LastName;
            }
            else
            {
                ViewBag.User = "Amira Mahmoud";
            }

            return View(appsList);
        }

        [HttpGet]
        public ActionResult ViewApplicant(long appId)
        {

            //Repositories
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            GenericRepository<PersonalDetail> userDetailsRepository = unitOfWork.GetRepoInstance<PersonalDetail>();
            GenericRepository<Applicant> applicantRepository = unitOfWork.GetRepoInstance<Applicant>();
            GenericRepository<Education> eduRepository = unitOfWork.GetRepoInstance<Education>();
            GenericRepository<WorkExperience> workRepository = unitOfWork.GetRepoInstance<WorkExperience>();
            GenericRepository<Applicant_Skill> skillRepository = unitOfWork.GetRepoInstance<Applicant_Skill>();
            GenericRepository<Applicant_Language> languageRepository = unitOfWork.GetRepoInstance<Applicant_Language>();
            GenericRepository<Application_Applicant> application_applicantRepository = unitOfWork.GetRepoInstance<Application_Applicant>();

            //Objects
            ApplicantDetailsViewModel model = new ApplicantDetailsViewModel();

            Applicant applicant = applicantRepository.GetBy(a => a.ApplicantID == appId).FirstOrDefault();
            User user = userRepository.GetBy(u => u.UserID == applicant.User.UserID).FirstOrDefault();
            PersonalDetail details = userDetailsRepository.GetBy(p => p.User.UserID == user.UserID).FirstOrDefault();
            Education edu = eduRepository.GetBy(e => e.ApplicantID == appId).FirstOrDefault();
            WorkExperience work = workRepository.GetBy(w => w.ApplicantID == appId).FirstOrDefault();
            var skill = skillRepository.GetBy(s => s.ApplicantID == appId).ToList();

            Applicant_Language languages = languageRepository.GetBy(l => l.ApplicantID == appId).FirstOrDefault();

            Application_Applicant application = application_applicantRepository.GetBy(a => a.ApplicantID == appId).OrderBy(a => a.SubmissionDate).LastOrDefault();

            //Add Data to The View Model

            model.ApplicantID = appId;
            model.UserID = user.UserID;
            model.Email = user.Email;
            model.FirstName = details.FirstName;
            model.LastName = details.LastName;
            model.Gender = details.Gender;
            model.DateOfBirth = details.DateOfBirth;
            model.Country = details.Country;
            model.PostalCode = details.PostalCode;
            model.TelephoneNumber = details.TelephoneNumber;
            model.MobileNumber = details.MobileNumber;
            model.Address = applicant.Address;
            model.CoverLetter = applicant.CoverLetter;
            model.Resume = applicant.Resume;
            model.WorkflowStatus = application.Status;
            model.Nationality = applicant.Nationality;
            model.City = applicant.City;
            model.Avatar = user.Avatar;

            model.Work = work;
            model.Edu = edu;
            model.Skill = new List<Applicant_Skill>();
            foreach (Applicant_Skill skills in skill)
            {
                model.Skill.Add(skills);
            }
            model.Languages = new List<Applicant_Language>();
            model.Languages.Add(languages);
            model.message = new Message();
            model.message.UserID = appId;
            model.message.SenderID = ((ProfileViewModel)Session["USER"]).UserID;

            if (Session["USER"] != null)
            {
                ProfileViewModel admin = (ProfileViewModel)Session["USER"];
                ViewBag.User = admin.FirstName + " " + admin.LastName;
            }
            else
            {
                ViewBag.User = "Amira Mahmoud";
            }

            return View("ApplicantDetails", model);
        }

        public ActionResult AuditTrails()
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<AuditTrail> auditRepository = unitOfWork.GetRepoInstance<AuditTrail>();
            List<AuditTrail> auditList = new List<AuditTrail>();

            var result = auditRepository.GetAll().OrderBy( a=>a.Time);
            foreach (AuditTrail audit in result)
            {
                auditList.Add(audit);
            }
            return View(auditList);
        }

        public ActionResult Accept(long appId)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            GenericRepository<Application_Applicant> applicationRepository = unitOfWork.GetRepoInstance<Application_Applicant>();

            Application_Applicant application = applicationRepository.GetBy(a => a.ApplicantID == appId).LastOrDefault();

            if (application.Status == 2)
            {
                application.Status = 3;
            }
            else if (application.Status == 3)
            {
                application.Status = 4;
            }
            else if (application.Status == 4)
            {
                application.Status = 5;
            }

            applicationRepository.Edit(application);
            unitOfWork.SaveChanges();

            return RedirectToAction("ViewApplicant", new { appId = appId });
        }
        public ActionResult Reject(long appId)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<Application_Applicant> applicationRepository = unitOfWork.GetRepoInstance<Application_Applicant>();
            Application_Applicant application = applicationRepository.GetBy(a => a.ApplicantID == appId).LastOrDefault();

            application.Status = 0;
            applicationRepository.Edit(application);
            unitOfWork.SaveChanges();

            return RedirectToAction("ViewApplicant", new { appId = appId });
        }

        public FileResult downloadFile(string FilePath)
        {
            string file = FilePath;
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(file, contentType, Path.GetFileName(file));
        }

    }
}