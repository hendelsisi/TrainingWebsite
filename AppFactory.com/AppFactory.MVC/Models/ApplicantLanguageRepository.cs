using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFactory.DAL;
namespace AppFactory.MVC.Models
{
    public class ApplicantLanguageRepository
    {
        private AppFactoryEntities context;

        public ApplicantLanguageRepository()
        {
            context = new AppFactoryEntities();
        }

        public ApplicantLanguageRepository(AppFactoryEntities db)
        {
            context = db;
        }

        public void Add(Applicant_Language applicantLanguage)
        {
            context.Applicant_Language.Add(applicantLanguage);
        }


        public void Save()
        {
            context.SaveChanges();
        }

        public int GetNumOfRecords()
        {
            int recordsNum = context.Applicant_Language.Count();
            return recordsNum;
        }
    }
}