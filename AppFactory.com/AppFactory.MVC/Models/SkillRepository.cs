using AppFactory.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppFactory.MVC.Models
{
    public class SkillRepository
    {
        private AppFactoryEntities db;
        public SkillRepository()
        {
            db = new AppFactoryEntities();
        }
        public SelectList GetLanguageslist()
        {
            SelectList skills = new SelectList(db.Skills, "SkillID", "SkillName");

            return skills;
        }

        public SelectList GetSkillList()
        {
            SelectList skills = new SelectList(db.Skills, "SkillID", "SkillName");

            return skills;
        }

    }
}