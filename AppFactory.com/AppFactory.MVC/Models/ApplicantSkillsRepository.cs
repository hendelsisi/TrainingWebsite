using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFactory.DAL;

namespace AppFactory.MVC.Models
{
    public class ApplicantSkillsRepository
    {
        private AppFactoryEntities context;
        public ApplicantSkillsRepository()
        {
            context = new AppFactoryEntities();
        }
        public ApplicantSkillsRepository(AppFactoryEntities db)
        {
            context = db;
        }

        public void Add(Applicant_Skill applicantSkill)
        {
            context.Applicant_Skill.Add(applicantSkill);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public int GetNumOfRecords()
        {
            int recordsNum = context.Applicant_Skill.Count();
            return recordsNum;
        }
    }
}