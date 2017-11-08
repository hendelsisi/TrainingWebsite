using System;
using System.Linq;
using System.Web;
using AppFactory.DAL;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AppFactory.MVC.Models
{
    public class CountryRepository
    {
        private AppFactoryEntities context;

        public CountryRepository(AppFactoryEntities db)
        {
            db = context;
        }

        public CountryRepository()
        {
            context = new AppFactoryEntities();
        }

        public SelectList GetCountriesInfo()
        {
            SelectList Countries = new SelectList(context.Countries, "CountryID", "CountryName");
            return Countries;
        }

        public Country GetRecordByID(int id)
        {
            Country country = context.Countries.Find(id);   
            return country;
        }
    }
}
