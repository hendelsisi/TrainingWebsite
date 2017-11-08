using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppFactory.DAL;
using AppFactory.MVC.ViewModels;
using AppFactory.MVC.Repositories;
using System.IO;
using AppFactory.MVC.Filters;
using System.Collections.Generic;
using AppFactory.MVC.Models;

namespace AppFactory.MVC.Controllers
{
    [AuditFilter]
    [LoggedInFilter]
    [VerifiedFilter]
    public class ApplicantController : Controller
    {
        GenericUnitOfWork unitOfWork;

        //Step One5
        [HttpGet]
        [ApplyFilter]
        public ActionResult ApplicationFormPersonalDetails()
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            long id = ((ProfileViewModel)Session["USER"]).UserID;

            //Get Country List From Database and send it to Register View
            GenericRepository<Country> countryRepository = unitOfWork.GetRepoInstance<Country>();
            SelectList countries = new SelectList(countryRepository.GetAll(), "CountryID", "CountryName");
            ViewBag.CountryList = countries;

            //ViewBag for Birth Year DropDownList Values
            SelectList birthDay = new SelectList(Enumerable.Range(1, (32 - 1)));
            ViewBag.BirthDay = birthDay;

            //ViewBag for Birth Year DropDownList Values
            SelectList birthMonth = new SelectList(Enumerable.Range(1, (13 - 1)));
            ViewBag.BirthMonth = birthMonth;

            //ViewBag for Birth Year DropDownList Values
            SelectList birthYear = new SelectList(Enumerable.Range(1980, (DateTime.Now.Year - 1980) + 1));
            ViewBag.BirthYear = birthYear;

            List<SelectListItem> Locations = new List<SelectListItem>()
            {
                new SelectListItem() {Text="Alexandria", Value="Alexandria"},
                new SelectListItem() { Text="Cairo", Value="Cairo"},
            };
            ViewBag.Locations = Locations;

            GenericRepository <PersonalDetail> userDetailRepository = unitOfWork.GetRepoInstance<PersonalDetail>();
            PersonalDetail personalDetail = userDetailRepository.GetBy(u => u.PersonalDetailID == id).FirstOrDefault();

            
            StepOneViewModel model = new StepOneViewModel()
            {
                UserID = id,
                FirstName = personalDetail.FirstName,
                LastName = personalDetail.LastName,
                Gender = personalDetail.Gender,
                Day = personalDetail.DateOfBirth.Day,
                Month = personalDetail.DateOfBirth.Month,
                Year = personalDetail.DateOfBirth.Year,
                MobileNumber = personalDetail.MobileNumber,
                CountryID = personalDetail.CountryID,
                PostalCode = personalDetail.PostalCode
            };

            return View(model);
        }

        //Step Two

        public ActionResult ApplicationFormEducation()
        {
            long applicantID = ((ProfileViewModel)Session["USER"]).UserID;
            StepTwoViewModel educationModel = new StepTwoViewModel();

            educationModel.ApplicantID = applicantID;


            //Get Colleges List From Database
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<College> collegeRepository = unitOfWork.GetRepoInstance<College>();
            SelectList colleges = new SelectList(collegeRepository.GetAll(), "CollegeID", "CollegeName");
            ViewBag.CollegeList = colleges;

            //ViewBag for Graduation Year DropDownList Values
            SelectList graduationYear = new SelectList(Enumerable.Range(2005, (DateTime.Now.Year - 2005) + 1));
            ViewBag.GradeYear = graduationYear;

            return View(educationModel);
        }

        //Step Three

        public ActionResult ApplicationFormWorkExperience()
        {
            long applicantID = ((ProfileViewModel)Session["USER"]).UserID;
            StepThreeViewModel workModel = new StepThreeViewModel();
            workModel.ApplicantID = applicantID;

            return View(workModel);
        }

        //Step Four

        public ActionResult ApplicationFormSkills(int id)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

            SkillModel skillsModel = new SkillModel();

            skillsModel.ApplicantID = id;
            skillsModel.firstSkillID = 1;
            skillsModel.firstSkilllevel = 1;

            //Get All Data From Skill repository
            GenericRepository<Skill> skillRepository = unitOfWork.GetRepoInstance<Skill>();
            SelectList skills = new SelectList(skillRepository.GetAll(), "SkillID", "SkillName");
            ViewBag.SkillsList = skills;

            return View(skillsModel);
        }

        //Step Five

        public ActionResult ApplicationFormLanguages()
        {
            long applicantID = ((ProfileViewModel)Session["USER"]).UserID;
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            StepFiveViewModel languagesModel = new StepFiveViewModel();
            languagesModel.ApplicantID = applicantID;


            //ViewBag for Languages DropDownList Values
            GenericRepository<Language> languageRepository = unitOfWork.GetRepoInstance<Language>();
            SelectList languages = new SelectList(languageRepository.GetAll(), "LanguageID", "languageName");
            ViewBag.LanguagesList = languages;

            return View();
        }



        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitApplicationFormPersonalDetails(StepOneViewModel model, HttpPostedFileBase ApplicantResume)
        {
            if (model != null)
            {
                unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];

                GenericRepository<PersonalDetail> personalDetailRepository = unitOfWork.GetRepoInstance<PersonalDetail>();
                GenericRepository<Applicant> applicantRepository = unitOfWork.GetRepoInstance<Applicant>();
                GenericRepository<Application_Applicant> application_applicantRepository = unitOfWork.GetRepoInstance<Application_Applicant>();
                GenericRepository<Application> applicationRepository = unitOfWork.GetRepoInstance<Application>();

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
                        }
                    }
                }

                //Update Personal-Details
                DateTime DOB = new DateTime(model.Year, model.Month, model.Day);

                PersonalDetail personalDetail = personalDetailRepository.GetBy(p => p.PersonalDetailID == model.UserID).FirstOrDefault();

                personalDetail.FirstName = model.FirstName;
                personalDetail.LastName = model.LastName;
                personalDetail.Gender = model.Gender;
                personalDetail.DateOfBirth = DOB;
                personalDetail.CountryID = model.CountryID;
                personalDetail.TelephoneNumber = model.TelephoneNumber;
                personalDetail.MobileNumber = model.MobileNumber;
                personalDetail.PostalCode = model.PostalCode;

                personalDetailRepository.Edit(personalDetail);

                
                //Add Record to Applicant
                Applicant applicantToAdd = new Applicant()
                {
                    ApplicantID = model.UserID,
                    Address = model.Address,
                    City = model.City,
                    Nationality = model.Nationality,
                    Resume = path,
                    CoverLetter = model.CoverLetter
                };

                if (applicantRepository.GetBy(a => a.ApplicantID == model.UserID).Count() == 0)
                {
                    applicantRepository.Add(applicantToAdd);
                }
                else
                {
                    Applicant applicant = applicantRepository
                        .GetBy(a => a.ApplicantID == model.UserID)
                        .FirstOrDefault();
                    applicant.Address = model.Address;
                    applicant.City = model.City;
                    applicant.Nationality = model.Nationality;
                    applicant.Resume = path;
                    applicant.CoverLetter = model.CoverLetter;

                    applicantRepository.Edit(applicant);
                }

                //Save all changes once.
                unitOfWork.SaveChanges();

                //Add record to Application_Applicant
                application_applicantRepository.Add(new Application_Applicant()
                {
                    ApplicantID = applicantToAdd.ApplicantID,
                    ApplicationID = applicationRepository.GetAll().LastOrDefault().ApplicationID,
                    Status = 1,
                    Step = 2,
                    SubmissionDate = DateTime.Now
                });

                unitOfWork.SaveChanges();
                //ChangeStep(applicantToAdd.ApplicantID, 2);
            }
            return RedirectToAction("ApplicationFormEducation", new { applicantID = model.UserID });
        }

        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitApplicationFormEducation(StepTwoViewModel model)
        {
            if (model != null)
            {
                unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
                GenericRepository<Education> educationRepository = unitOfWork.GetRepoInstance<Education>();

                if (educationRepository.GetBy(e => e.ApplicantID == model.ApplicantID).Count() == 0)
                {
                    Education education = new Education()
                    {
                        ApplicantID = model.ApplicantID,
                        CollegeID = model.CollegeID,
                        Degree = model.Degree,
                        GeaduationYear = model.GeaduationYear,
                        Grade = model.Grade
                    };

                    educationRepository.Add(education);
                }
                else
                {
                    Education education = educationRepository.GetBy(e => e.ApplicantID == model.ApplicantID).FirstOrDefault();
                    education.CollegeID = model.CollegeID;
                    education.Degree = model.Degree;
                    education.GeaduationYear = model.GeaduationYear;
                    education.Grade = model.Grade;

                    educationRepository.Edit(education);
                }

                unitOfWork.SaveChanges();
                
                ChangeStep(model.ApplicantID,  3);
                
            }
            return RedirectToAction("ApplicationFormWorkExperience", new { applicantID = model.ApplicantID });
        }

        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitApplicationFormWorkExperience(StepThreeViewModel model)
        {
            if (model != null)
            {
                unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
                GenericRepository<WorkExperience> workExperienceRepository = unitOfWork.GetRepoInstance<WorkExperience>();
                if (workExperienceRepository.GetBy(w => w.ApplicantID == model.ApplicantID).Count() == 0)
                {
                    WorkExperience workExperience = new WorkExperience()
                    {
                        ApplicantID = model.ApplicantID,
                        CompanyName = model.CompanyName,
                        Position = model.Position,
                        Responsibility = model.Responsibility,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    };
                    workExperienceRepository.Add(workExperience);
                }
                else
                {
                    WorkExperience workExperience = workExperienceRepository.GetBy(w => w.ApplicantID == model.ApplicantID).FirstOrDefault();
                    workExperience.CompanyName = model.CompanyName;
                    workExperience.Position = model.Position;
                    workExperience.Responsibility = model.Responsibility;
                    workExperience.StartDate = model.StartDate;
                    workExperience.EndDate = model.EndDate;

                    workExperienceRepository.Edit(workExperience);
                }
                unitOfWork.SaveChanges();

                ChangeStep(model.ApplicantID, 4);
            }
            return RedirectToAction("ApplicationFormSkills", new { id = model.ApplicantID });
        }

        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitApplicationFormSkills(SkillModel model)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<Applicant_Skill> applicantSkillsRepository = unitOfWork.GetRepoInstance<Applicant_Skill>();
            GenericRepository<Skill> SkillsRepository = unitOfWork.GetRepoInstance<Skill>();

            //TODO: Make it multiple skills.
            /////////////////////
            if (applicantSkillsRepository.GetBy(l => l.ApplicantID == model.ApplicantID).Count() == 0)
            {

                applicantSkillsRepository.Add(new Applicant_Skill()
                {
                    ApplicantID = model.ApplicantID,
                    SkillID = model.firstSkillID,

                    SkillLevel = model.firstSkilllevel
                });

                for (int i = 0; i < model.NewSkill.Count(); i++)
                {
                    applicantSkillsRepository.Add(new Applicant_Skill()
                    {
                        ApplicantID = model.ApplicantID,
                        SkillID = model.NewSkill[i].SkillID,
                        SkillLevel = model.NewSkill[i].SkillLevel
                    });
                }
            }
            else
            {
                applicantSkillsRepository.Edit(new Applicant_Skill()
                {
                    ApplicantID = model.ApplicantID,
                    SkillID = model.firstSkillID,
                    SkillLevel = model.firstSkilllevel
                });

                for (int i = 0; i < model.NewSkill.Count(); i++)
                {
                    applicantSkillsRepository.Edit(new Applicant_Skill()
                    {
                        ApplicantID = model.ApplicantID,
                        SkillID = model.NewSkill[i].SkillID,
                        SkillLevel = model.NewSkill[i].SkillLevel
                    });
                }

            }
            /////////////////

           

            unitOfWork.SaveChanges();
            return RedirectToAction("ApplicationFormLanguages", new { applicantID = model.ApplicantID });
        }

        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitApplicationFormLanguages(Applicant_Language applicantLanguage)
        {
            if (applicantLanguage != null)
            {
                unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
                GenericRepository<Applicant_Language> applicantLanguageRepository = unitOfWork.GetRepoInstance<Applicant_Language>();
                GenericRepository<Application_Applicant> application_applicantRepository = unitOfWork.GetRepoInstance<Application_Applicant>();
                GenericRepository<Language> languageRepository = unitOfWork.GetRepoInstance<Language>();

                long appId = ((ProfileViewModel)Session["USER"]).UserID;
                Applicant_Language language = applicantLanguageRepository.GetBy(l => l.ApplicantID == appId).FirstOrDefault();
                if (applicantLanguageRepository.GetBy(l => l.ApplicantID == appId).Count() == 0)
                {
                    //for (int i = 0; i < applicantLanguages.Length; i++)
                    //{
                    Applicant_Language languageToAdd = new Applicant_Language
                    {
                        ApplicantID = appId,
                        LanguageID = applicantLanguage.ID,
                        Language = languageRepository.GetBy(l => l.LanguageID == applicantLanguage.LanguageID).FirstOrDefault(),
                        SpokenLevel = applicantLanguage.SpokenLevel,
                        WrittenLevel = applicantLanguage.WrittenLevel
                    };
                    applicantLanguageRepository.Add(languageToAdd);
                    //}
                }
                else
                {
                    
                    language.ApplicantID = appId;
                    language.LanguageID = applicantLanguage.LanguageID;
                    language.Language = languageRepository.GetBy(l => l.LanguageID == applicantLanguage.LanguageID).FirstOrDefault();
                    language.SpokenLevel = applicantLanguage.SpokenLevel;
                    language.WrittenLevel = applicantLanguage.WrittenLevel;

                    applicantLanguageRepository.Edit(language);
                }
                unitOfWork.SaveChanges();

                //Application_Applicant application = application_applicantRepository
                //    .GetBy(a => a.ApplicantID == applicantLanguage.ApplicantID)
                //    .OrderBy(a => a.SubmissionDate)
                //    .LastOrDefault();
                //application.Status = 2;
                //unitOfWork.SaveChanges(); ??????????????????????????

                ChangeStep(appId, 6);
            }
            return View("ApplicationSubmitted");
        }

        private void ChangeStep(long id, int step)
        {
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<Application_Applicant> appRepository = unitOfWork.GetRepoInstance<Application_Applicant>();

            Application_Applicant updatedApp = appRepository
                .GetBy(u => u.ApplicantID == id)
                .OrderBy(a => a.SubmissionDate)
                .LastOrDefault();
            
            if (updatedApp != null)
            {
                updatedApp.Step = (int)step;
                appRepository.Edit(updatedApp);
                unitOfWork.SaveChanges();
            }
            int x = updatedApp.Step;
        }
        public ActionResult CreateNewSkill()
        {
            var UserSkill = new UserSkill();
            unitOfWork = (GenericUnitOfWork)Session["UNITOFWORK"];
            GenericRepository<Skill> skillRepository = unitOfWork.GetRepoInstance<Skill>();
            SelectList skills = new SelectList(skillRepository.GetAll(), "SkillID", "SkillName");
            ViewBag.SkillsList = skills;

            return PartialView("~/Views/Shared/EditorTemplates/SkillViewModel.cshtml", UserSkill);

        }
    }
}
