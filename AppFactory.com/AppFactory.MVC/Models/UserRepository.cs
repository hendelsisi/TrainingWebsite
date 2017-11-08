using System.Linq;
using AppFactory.DAL;
using AppFactory.Security;
using System;

namespace AppFactory.MVC.Models
{
    public class UserRepository
    {
        private AppFactoryEntities context;

        public UserRepository()
        {
            context = new AppFactoryEntities();
        }

        /// <summary>
        /// Class constructor with dbcontext initialization.
        /// </summary>
        /// <param name="db">The database context object</param>
        public UserRepository(AppFactoryEntities db)
        {
            context = db;
        }

        public void Add(User user)
        {
            context.Users.Add(user);
        }
        
        public void Save()
        {
            context.SaveChanges();
        }

        //public void ConfirmUserEmail(int userId)
        //{
        //    User UpdatedUser = new User()
        //    {
        //        EmailConfirmed = true,
        //        UserID = userId
        //    };
        //    context.Users.Attach(UpdatedUser);
        //    var entry = context.Entry(UpdatedUser);
        //    entry.Property(e => e.EmailConfirmed).IsModified = true;
        //    context.SaveChanges();
        //}

        //Add Record in Role_User Table using user's ID
        public void AddRole(long userId, int roleId)
        {
            RoleUserRepository roleUserRepository = new RoleUserRepository();
            Role_User roleUser = new Role_User()
            {
                UserID = userId,
                RoleID = roleId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            roleUserRepository.Add(roleUser);
            roleUserRepository.Save();
        }
        
        public User IsValidUser(string email, string password)
        {
            User user;
            var result = (from usr in context.Users
                          where usr.Email == email
                          select usr);

            if (result.Count() != 0)
            {
                user = result.First();

                string decryptedPassword = Cryptography.Decrypt(user.Password, user.Salt);

                if (decryptedPassword == password)
                {
                    return user;
                }
            }

            return null;
        }

    }
}