using AppFactory.DAL;
using System.Linq;

namespace AppFactory.MVC.Models
{
    public class PersonalDetailsRepository
    {
        private AppFactoryEntities context;

        public PersonalDetailsRepository()
        {
            context = new AppFactoryEntities();
        }

        /// <summary>
        /// Class constructor with dbcontext initialization.
        /// </summary>
        /// <param name="db">The database context object</param>
        public PersonalDetailsRepository(AppFactoryEntities db)
        {
            context = db;
        }

        public void Add(PersonalDetail personalDetails)
        {
            context.PersonalDetails.Add(personalDetails);
        }
        
        public void Save()
        {
            context.SaveChanges();
        }


        public PersonalDetail FindRecord(long userId)
        {
            //var data = context.PersonalDetails.SqlQuery("Select * from [dbo].[PersonalDetails] Where UserID = " + userId + ";");
            var data = context.PersonalDetails.Where(p => p.PersonalDetailID == userId);
            PersonalDetail userDetails = data.First();
            return userDetails;
        }

    }
}