using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFactory.DAL;

namespace AppFactory.MVC.Models
{
    public class ApplicantRepository
    {
        private AppFactoryEntities context;

        public ApplicantRepository()
        {
            context = new AppFactoryEntities();
        }

        /// <summary>
        /// Class constructor with dbcontext initialization.
        /// </summary>
        /// <param name="db">The database context object</param>
        public ApplicantRepository(AppFactoryEntities db)
        {
            context = db;
        }

        public void Add(Applicant applicant)
        {
            context.Applicants.Add(applicant);
        }


        public void Save()
        {
            context.SaveChanges();
        }

        public int GetNumOfUsers()
        {
            int usersNum = context.Users.Count();
            return usersNum;
        }

    }
}