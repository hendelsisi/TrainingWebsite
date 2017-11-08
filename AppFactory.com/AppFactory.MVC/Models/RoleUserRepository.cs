using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFactory.DAL;

namespace AppFactory.MVC.Models
{
    public class RoleUserRepository
    {
        private AppFactoryEntities context;

        public RoleUserRepository()
        {
            context = new AppFactoryEntities();
        }

        /// <summary>
        /// Class constructor with dbcontext initialization.
        /// </summary>
        /// <param name="db">The database context object</param>
        public RoleUserRepository(AppFactoryEntities db)
        {
            context = db;
        }

        public void Add(Role_User roleUser)
        {
            context.Role_User.Add(roleUser);
        }


        public void Save()
        {
            context.SaveChanges();
        }
        
    }
}