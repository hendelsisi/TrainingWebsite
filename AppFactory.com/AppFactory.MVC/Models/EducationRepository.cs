using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFactory.DAL;

namespace AppFactory.MVC.Models
{
    public class EducationRepository
    {
        private AppFactoryEntities context;
        
        public EducationRepository()
        {
            context = new AppFactoryEntities();
        }   
        public EducationRepository(AppFactoryEntities db)
        {
            context = db;
        }
        public void Add(Education education)
        {
            context.Educations.Add(education);
        }


        public void Save()
        {
            context.SaveChanges();
        }

        public int GetNumOfRecords()
        {
            int recordsNum = context.Educations.Count();
            return recordsNum;
        }
    }
}