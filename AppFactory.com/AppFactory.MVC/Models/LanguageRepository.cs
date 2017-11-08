using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFactory.DAL;
using System.Web.Mvc;

namespace AppFactory.MVC.Models
{
    public class LanguageRepository
    {
        private AppFactoryEntities db;
        public LanguageRepository()
        {
            db = new AppFactoryEntities();
        }
        public SelectList GetLanguageslist()
        {
            SelectList languages = new SelectList(db.Languages, "LanguageID", "languageName");

            return languages;
        }
    }
}