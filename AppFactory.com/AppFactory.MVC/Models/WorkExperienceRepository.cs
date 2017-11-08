using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFactory.DAL;

namespace AppFactory.MVC.Models
{
    public class WorkExperienceRepository
    {
        private AppFactoryEntities context;
        public WorkExperienceRepository()
        {
            context = new AppFactoryEntities();
        }
        public WorkExperienceRepository(AppFactoryEntities db)
        {
            context = db;
        }

        public void Add(WorkExperience workExperience)
        {
            context.WorkExperiences.Add(workExperience);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public int GetNumOfRecords()
        {
            int recordsNum = context.WorkExperiences.Count();
            return recordsNum;
        }
    }
}