using AppFactory.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppFactory.MVC.Models
{
    public class CollegeRepository
    {
        private AppFactoryEntities context;

        public CollegeRepository(AppFactoryEntities db)
        {
            db = context;
        }
        public CollegeRepository()
        {
            context = new AppFactoryEntities();
        }
        public SelectList GetCollegesInfo()
        {
            SelectList Colleges = new SelectList(context.Colleges, "CollegeID", "CollegeName");
            return Colleges;
        }
    }
}