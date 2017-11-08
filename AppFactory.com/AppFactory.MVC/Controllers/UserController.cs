using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppFactory.Mailing;
using AppFactory.MVC.ViewModels;
using AppFactory.MVC.Repositories;
using AppFactory.MVC.Filters;
using AppFactory.DAL;
using AppFactory.Security;
using System.IO;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Collections.Generic;

namespace AppFactory.MVC.Controllers
{
    public class UserController : Controller
    {
        GenericUnitOfWork unitOfWork;

        [NonLoggedInFilter]
        public ActionResult Login()
        {
            return View();
        }

        [LoggedInFilter]
        public ActionResult Logout()
        {
            ProfileViewModel sessionUser = (ProfileViewModel)Session["USER"];

            // Make that user offline.
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            User user = userRepository.GetBy(u => u.UserID == sessionUser.UserID).FirstOrDefault();
            user.Online = false;
            userRepository.Edit(user);
            unitOfWork.SaveChanges();

            if (user.ExternLogin == true)
            {
                SignOut();
            }

            Session["USER"] = null;

            //Update Number of LoggedIn Users
            int value = (int)System.Web.HttpContext.Current.Application["LoggedInUsers"] - 1;
            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application["LoggedInUsers"] = value;
            System.Web.HttpContext.Current.Application.UnLock();

            return RedirectToAction("Index", "Home", null);
        }

        [LoggedInFilter]
        public ActionResult UserProfile()
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            ProfileViewModel sessionUser = (ProfileViewModel)Session["USER"];

            GenericRepository<Applicant> appRepository = unitOfWork.GetRepoInstance<Applicant>();
            GenericRepository<Application_Applicant> hisoryRepository = unitOfWork.GetRepoInstance<Application_Applicant>();
            GenericRepository<Message> messageRepository = unitOfWork.GetRepoInstance<Message>();

            Applicant applicant = appRepository
                .GetBy(a => a.User.UserID == sessionUser.UserID).FirstOrDefault();


            if (applicant != null)
            {
                List<Application_Applicant> applications = hisoryRepository
                    .GetBy(a => a.ApplicantID == applicant.ApplicantID).ToList();

                Application_Applicant currentApplication = applications
                    .OrderBy(a => a.SubmissionDate)
                    .LastOrDefault();

                ApplicantViewModel appModel = new ApplicantViewModel()
                {
                    UserID = sessionUser.UserID,
                    ApplicantID = applicant.ApplicantID,
                    AppStatus = currentApplication.Status,
                    Step = currentApplication.Step,
                    History = applications
                };

                sessionUser.model = appModel;
            }

            sessionUser.Messages = messageRepository.GetBy(m => m.UserID == sessionUser.UserID).ToList();

            return View(sessionUser);
        }

        public ActionResult EditProfile()
        {
            ProfileViewModel modelview = (ProfileViewModel)Session["USER"];

            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            //Get Country List From Database and send it to Register View
            GenericRepository<Country> countryRepository = unitOfWork.GetRepoInstance<Country>();
            SelectList countries = new SelectList(countryRepository.GetAll(), "CountryID", "CountryName");
            ViewBag.CountryList = countries;

            ProfileViewModel mode = new ProfileViewModel()
            {
                UserID = modelview.UserID,
                FirstName = modelview.FirstName,
                LastName = modelview.LastName,
                Gender = modelview.Gender,
                MobileNumber = modelview.MobileNumber,
                PostalCode = modelview.PostalCode,
                CountryID = modelview.CountryID
            };
            return View(mode);
        }

        [HttpPost]
        public ActionResult EditProfile(ProfileViewModel model)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            long id = ((ProfileViewModel)Session["USER"]).UserID;

            User user_to_update = userRepository.GetBy(u => u.UserID == id).FirstOrDefault();
            user_to_update.PersonalDetail.FirstName = model.FirstName;
            user_to_update.PersonalDetail.LastName = model.LastName;
            user_to_update.PersonalDetail.Gender = model.Gender;
            user_to_update.PersonalDetail.DateOfBirth = model.DateOfBirth;
            user_to_update.PersonalDetail.MobileNumber = model.MobileNumber;
            user_to_update.PersonalDetail.PostalCode = model.PostalCode;
            user_to_update.PersonalDetail.CountryID = model.CountryID;

            userRepository.Edit(user_to_update);
            unitOfWork.SaveChanges();

            ProfileViewModel proVM = (ProfileViewModel)Session["USER"];

            proVM.CountryID = model.CountryID;
            proVM.Gender = model.Gender;
            //proVM.Day = model.DateOfBirth.Day;
            //proVM.Month = model.DateOfBirth.Month;
            //proVM.Year = model.DateOfBirth.Year;
            proVM.MobileNumber = model.MobileNumber;
            proVM.FirstName = model.FirstName;
            proVM.LastName = model.LastName;
            proVM.PostalCode = model.PostalCode;

            Session["USER"] = proVM;
            return View("UserProfile", proVM);
        }

        [NonLoggedInFilter]
        [HttpPost]
        public ActionResult Login(string email, string password, bool RememberMe)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();

            var result = userRepository.GetBy(u => u.Email == email);

            //1. Check for email.
            if (result.Count() != 0)
            {
                User testedUser = result.FirstOrDefault();

                //2. Check for lock.
                if (testedUser.IsLocked)
                {
                    ViewBag.Error = "Your Acccount Has Been Locked For Five Minutes , Try Logging in after that.";
                }
                else //Check for lock
                {
                    //3. Check for password.
                    string decryptedPassword = Cryptography.Decrypt(testedUser.Password, testedUser.Salt);
                    if (decryptedPassword == password)
                    {
                        // No error messages.
                        ViewBag.Error = null;

                        //Reset invalid attempts.
                        if (testedUser.InvalidAttempts > 0)
                        {
                            testedUser.InvalidAttempts = 0;
                            userRepository.Edit(testedUser);
                            unitOfWork.SaveChanges();
                        }

                        //Map the logged in user data to ProfileViewModel.
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

                        //Save user data in the current session.
                        Session["USER"] = proVM;

                        // Update Number Of Logged in users
                        int value = (int)System.Web.HttpContext.Current.Application["LoggedInUsers"] + 1;

                        System.Web.HttpContext.Current.Application.Lock();
                        System.Web.HttpContext.Current.Application["LoggedInUsers"] = value;
                        System.Web.HttpContext.Current.Application.UnLock();

                        //Check if user want a remember me cookie.
                        if (RememberMe)
                        {
                            testedUser.RememberToken = Cryptography.GetRandomKey(8);
                            userRepository.Edit(testedUser);
                            unitOfWork.SaveChanges();

                            HttpCookie LoginCookie = new HttpCookie("LoginCookie");
                            LoginCookie.Values.Add("RemeberMe", testedUser.RememberToken);
                            LoginCookie.Values.Add("UserID", testedUser.UserID.ToString());
                            LoginCookie.Expires = DateTime.Now.AddDays(30);
                            Response.Cookies.Set(LoginCookie);
                        }

                        return RedirectToAction("UserProfile", "User");
                    }
                    else //Check for password
                    {
                        if (testedUser.InvalidAttempts < 3)
                        {
                            testedUser.InvalidAttempts += 1;
                            unitOfWork.SaveChanges();
                            ViewBag.Error = "Incorrect Email or Password";
                        }
                        else
                        {
                            testedUser.InvalidAttempts = 0;
                            testedUser.IsLocked = true;
                            testedUser.LockEnd = DateTime.Now.AddMinutes(5);
                            unitOfWork.SaveChanges();
                            ViewBag.Error = "Your Acccount Has Been Locked For Five Minutes , Try Logging in after that.";
                        }
                    }
                }
            }
            else //Check for email
            {
                ViewBag.Error = "Incorrect Email or Password";
            }

            return View();
        }

        [NonLoggedInFilter]
        public ActionResult Register()
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            //Get Country List From Database and send it to Register View
            GenericRepository<Country> countryRepository = unitOfWork.GetRepoInstance<Country>();
            SelectList countries = new SelectList(countryRepository.GetAll(), "CountryID", "CountryName");
            ViewBag.CountryList = countries;

            //ViewBag for Birth Day DropDownList Values
            SelectList birthDay = new SelectList(Enumerable.Range(1, (32 - 1)));
            ViewBag.BirthDay = birthDay;

            //ViewBag for Birth Month DropDownList Values
            SelectList birthMonth = new SelectList(Enumerable.Range(1, (13 - 1)));
            ViewBag.BirthMonth = birthMonth;

            //ViewBag for Birth Year DropDownList Values
            SelectList birthYear = new SelectList(Enumerable.Range(1980, (DateTime.Now.Year - 1980) + 1));
            ViewBag.BirthYear = birthYear;

            return View();
        }

        [NonLoggedInFilter]
        [ActionName("MicrosoftRegister")]
        public ActionResult Register(RegisterationViewModel model)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            //Get Country List From Database and send it to Register View
            GenericRepository<Country> countryRepository = unitOfWork.GetRepoInstance<Country>();
            SelectList countries = new SelectList(countryRepository.GetAll(), "CountryID", "CountryName");
            ViewBag.CountryList = countries;

            //ViewBag for Birth Day DropDownList Values
            SelectList birthDay = new SelectList(Enumerable.Range(1, (32 - 1)));
            ViewBag.BirthDay = birthDay;

            //ViewBag for Birth Month DropDownList Values
            SelectList birthMonth = new SelectList(Enumerable.Range(1, (13 - 1)));
            ViewBag.BirthMonth = birthMonth;

            //ViewBag for Birth Year DropDownList Values
            SelectList birthYear = new SelectList(Enumerable.Range(1980, (DateTime.Now.Year - 1980) + 1));
            ViewBag.BirthYear = birthYear;

            return View("CompleteRegisteration", model);
        }

        [NonLoggedInFilter]
        [HttpPost]
        public ActionResult ConfirmSubmission(RegisterationViewModel model, HttpPostedFileBase UserAvatar)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            GenericRepository<PersonalDetail> personalDetailsRepository = unitOfWork.GetRepoInstance<PersonalDetail>();

            var path = "";
            if (UserAvatar != null)
            {
                if (UserAvatar.ContentLength > 0)
                {
                    if (Path.GetExtension(UserAvatar.FileName).ToLower() == ".jpg"
                        || Path.GetExtension(UserAvatar.FileName).ToLower() == ".png"
                        || Path.GetExtension(UserAvatar.FileName).ToLower() == ".gif"
                        || Path.GetExtension(UserAvatar.FileName).ToLower() == ".jpeg")
                    {
                        string imageHash = Cryptography.GetRandomKey(8);
                        path = Path.Combine(Server.MapPath("~/images/avatar/"), imageHash + "_" + UserAvatar.FileName);
                        UserAvatar.SaveAs(path);
                        path = "images/avatar/" + imageHash + "_" + UserAvatar.FileName;
                    }
                }
            }
            else
            {
                if (model.Gender)
                {
                    path = "images/avatar/avatar-male.png";
                }
                else
                {
                    path = "images/avatar/avatar-female.png";
                }
            }

            //Add Data to User Table.
            string passHash = Cryptography.GetRandomKey(64);
            User user = new User()
            {
                Email = model.Email,
                Salt = passHash,
                Password = Cryptography.Encrypt(model.Password, passHash),
                VerificationCode = Cryptography.GetRandomKey(8),
                Avatar = path,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,

                RoleID = 2,
                EmailConfirmed = false,
                Active = true,
                Online = true,
                IsLocked = false,
                ExternLogin = false,
                InvalidAttempts = 0
            };

            if (model.ExternLogin == true)
                user.ExternLogin = true;
            else
                user.ExternLogin = false;

            //Change 1/2
            userRepository.Add(user);

            //Add Data to PersonalDetails Table
            DateTime DOB = new DateTime(model.Year, model.Month, model.Day);
            PersonalDetail userDetails = new PersonalDetail()
            {
                PersonalDetailID = user.UserID,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                DateOfBirth = DOB,
                CountryID = model.CountryID,
                MobileNumber = model.MobileNumber,
                PostalCode = model.PostalCode
            };

            //Change 2/2
            personalDetailsRepository.Add(userDetails);

            //Save all changes.
            unitOfWork.SaveChanges();

            // Update Number Of Logged in users
            int value = (int)System.Web.HttpContext.Current.Application["LoggedInUsers"] + 1;

            System.Web.HttpContext.Current.Application.Lock();
            System.Web.HttpContext.Current.Application["LoggedInUsers"] = value;
            System.Web.HttpContext.Current.Application.UnLock();

            //Send confirmation email
            try
            {
                if (ModelState.IsValid)
                {

                    string body = string.Format("Dear {0} , <br/> Thank you for your registration, please click on the below link to complete your registration:<a href=\"{1}\"title=\"User Email Confirm\">{1}</a>",
                        userDetails.FirstName + " " + userDetails.LastName,
                        Url.Action("ConfirmEmail", "User", new { Token = user.VerificationCode, Email = user.Email }, Request.Url.Scheme));

                    Mailing.EmailConfig conf = new Mailing.EmailConfig("amirademo89@gmail.com", "AppFactory.com demo", "demowebsite")
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true
                    };

                    Email email = new Email(conf);
                    email.Send(user.Email, "Confirm Your Email", body);
                }
            }
            catch (Exception)
            {

            }

            ProfileViewModel proVM = new ProfileViewModel()
            {
                UserID = user.UserID,
                Avatar = user.Avatar,
                Email = user.Email,

                PostalCode = userDetails.PostalCode,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                CountryID = userDetails.CountryID,
                Country = userDetails.Country,
                Gender = userDetails.Gender,
                MobileNumber = userDetails.MobileNumber,
                DateOfBirth = userDetails.DateOfBirth
            };

            Session["USER"] = proVM;

            return View();
        }

        public ActionResult ConfirmEmail()
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();

            string email = Request.QueryString["Email"].ToString();
            var users = userRepository.GetBy(u => u.Email == email);
            if (users.Count() != 0)
            {
                User user = users.FirstOrDefault();
                if (user.VerificationCode == Request.QueryString["Token"].ToString())
                {
                    user.EmailConfirmed = true;
                    unitOfWork.SaveChanges();
                    ViewBag.Message = "Your email has verified successfully";
                }
                else
                {
                    ViewBag.Message = "Invalid Verification";
                }
            }

            return View();
        }

        [LoggedInFilter]
        [HttpPost]
        public ActionResult ChangeProfilePhoto(HttpPostedFileBase updatedPhoto)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();

            var path = "";
            if (updatedPhoto != null)
            {
                if (updatedPhoto.ContentLength > 0)
                {
                    if (Path.GetExtension(updatedPhoto.FileName).ToLower() == ".jpg"
                        || Path.GetExtension(updatedPhoto.FileName).ToLower() == ".png"
                        || Path.GetExtension(updatedPhoto.FileName).ToLower() == ".gif"
                        || Path.GetExtension(updatedPhoto.FileName).ToLower() == ".jpeg")
                    {
                        string imageHash = Cryptography.GetRandomKey(8);
                        path = Path.Combine(Server.MapPath("~/images/avatar/"), imageHash + "_" + updatedPhoto.FileName);
                        updatedPhoto.SaveAs(path);
                        path = "images/avatar/" + imageHash + "_" + updatedPhoto.FileName;
                    }
                }
            }

            ProfileViewModel proVM = (ProfileViewModel)Session["USER"];
            long id = proVM.UserID;

            User UpdatedUser = userRepository.GetBy(u => u.UserID == id).FirstOrDefault();

            UpdatedUser.Avatar = path;
            userRepository.Edit(UpdatedUser);
            unitOfWork.SaveChanges();

            proVM.Avatar = path;

            return View("UserProfile", proVM);
        }

        [HttpGet]
        [NonLoggedInFilter]
        public ActionResult ForgetPasswd()
        {
            return View();
        }

        [HttpPost]
        [NonLoggedInFilter]
        public ActionResult ForgetPasswd(string email)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            GenericRepository<PersonalDetail> personalDetailsRepository = unitOfWork.GetRepoInstance<PersonalDetail>();

            User user = userRepository.GetBy(u => u.Email == email).FirstOrDefault();
            PersonalDetail userDetails;

            if (user != null)
            {
                userDetails = personalDetailsRepository.GetBy(p => p.User.UserID == user.UserID).FirstOrDefault();

                try
                {
                    if (ModelState.IsValid)
                    {

                        string body = string.Format("Dear {0} , <br/> Please Use the link below to Reset Your Password : \n" +
                            "<a href=\"{1}\"title=\"User Email Confirm\">{1}</a>",
                            userDetails.FirstName + " " + userDetails.LastName,
                            Url.Action("ResetPassword", "User", new { Token = user.VerificationCode, Email = user.Email }, Request.Url.Scheme));

                        Mailing.EmailConfig conf = new Mailing.EmailConfig("amirademo89@gmail.com", "AppFactory.com demo", "demowebsite")
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true
                        };

                        Email uEmail = new Email(conf);
                        uEmail.Send(user.Email, "Reset Password", body);
                    }
                }
                catch (Exception)
                {

                }
                ViewBag.UserEmail = email;
                return View("ResetPassMessage");
            }
            else
            {
                ViewBag.ErrorMessage = "The email you entered is not registered please try again.";
                return View();
            }
        }

        [HttpGet]
        [NonLoggedInFilter]
        public ActionResult ResetPassword(string Token, string Email)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            User user = userRepository.GetBy(u => u.Email == Email).FirstOrDefault();
            if (user.VerificationCode == Token)
            {
                ResetPasswd model = new ResetPasswd();
                model.UserID = user.UserID;
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [NonLoggedInFilter]
        public ActionResult ResetPassword(ResetPasswd model)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            User user = userRepository.GetBy(u => u.UserID == model.UserID).FirstOrDefault();
            user.Password = Cryptography.Encrypt(model.NewPassword, user.Salt);
            user.VerificationCode = Cryptography.GetRandomKey(8);
            user.UpdatedAt = DateTime.Now;
            userRepository.Edit(user);
            unitOfWork.SaveChanges();

            return RedirectToAction("Login");
        }

        [LoggedInFilter]
        public ActionResult ChangePassword(long userId)
        {

            ChangePassword model = new ChangePassword();
            model.UserID = userId;
            return View(model);
        }

        [LoggedInFilter]
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword model)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();
            User user = userRepository.GetBy(u => u.UserID == model.UserID).FirstOrDefault();

            if (user != null)
            {
                string currentPass = Cryptography.Decrypt(user.Password, user.Salt);
                if (currentPass == model.CurrentPassword)
                {
                    string hash = Cryptography.GetRandomKey(64);
                    user.Salt = hash;
                    user.Password = Cryptography.Encrypt(model.NewPassword, hash);
                    user.UpdatedAt = DateTime.Now;
                    userRepository.Edit(user);
                    unitOfWork.SaveChanges();
                }
                else
                {

                    TempData["Error"] = "Incorrect Password , Try Again!";
                    return RedirectToAction("ChangePassword", new { userId = model.UserID });
                }
            }
            return RedirectToAction("UserProfile");
        }

        [LoggedInFilter]
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
            model.Applicant = applicant;
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

            return View("ApplicantDetails", model);
        }

        public JsonResult IsValidEmail(string Email)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<User> userRepository = unitOfWork.GetRepoInstance<User>();

            if (userRepository.GetBy(u => u.Email == Email).Count() == 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Send an OpenID Connect sign-in request.
        /// Alternatively, you can just decorate the SignIn method with the [Authorize] attribute
        /// </summary>
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            //HttpContext.GetOwinContext().Authentication.SignOut(
            //        OpenIdConnectAuthenticationDefaults.AuthenticationType,
            //        CookieAuthenticationDefaults.AuthenticationType);
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            Response.Redirect("http://localhost:59021/");
        }

        public FileResult downloadFile(string FilePath)
        {
            string file = FilePath;
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }

        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCV(Applicant applicant , HttpPostedFileBase ApplicantResume)
        {
            long i = applicant.ApplicantID;
            var path = "";
            if (ApplicantResume != null)
            {
                if (ApplicantResume.ContentLength > 0)
                {
                    if (Path.GetExtension(ApplicantResume.FileName).ToLower() == ".doc"
                        || Path.GetExtension(ApplicantResume.FileName).ToLower() == ".docx"
                        || Path.GetExtension(ApplicantResume.FileName).ToLower() == ".pdf")
                    {

                        path = Path.Combine(Server.MapPath("~/App_Data/Users_Content/"), ApplicantResume.FileName);
                        ApplicantResume.SaveAs(path);
                        ViewBag.Error = "";
                    }
                    else
                    {
                        ViewBag.Error = "You can only upload files with .doc , .docx or .pdf extensions";
                        return RedirectToAction("ViewApplicant" , new { appId = applicant.ApplicantID});
                    }
                }
            }
            else
            {
                ViewBag.Error = "You must choose a file to upload";
                return RedirectToAction("ViewApplicant", new { appId = applicant.ApplicantID });
            }
            
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<Applicant> appRepository = unitOfWork.GetRepoInstance<Applicant>();

            Applicant updatedApp = appRepository.GetBy(a => a.ApplicantID == applicant.ApplicantID).FirstOrDefault();
            updatedApp.Resume = path;
            appRepository.Edit(updatedApp);
            unitOfWork.SaveChanges();

            return RedirectToAction("ViewApplicant" , new { appId = applicant.ApplicantID });
        }

        [HttpPost]
        public ActionResult SendMessage(Message message)
        {
            long temp;
            temp = message.SenderID;
            message.SenderID = message.UserID;
            message.UserID = temp;

            message.SentDate = DateTime.Now;
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<Message> messageRepository = unitOfWork.GetRepoInstance<Message>();
            messageRepository.Add(message);

            unitOfWork.SaveChanges();

            return RedirectToAction("UserProfile");
        }

    }
}
