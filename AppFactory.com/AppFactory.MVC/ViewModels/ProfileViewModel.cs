using AppFactory.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppFactory.MVC.ViewModels
{
    public class ProfileViewModel
    {
        [Key]
        public long UserID;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MobileNumber { get; set; }
        public int CountryID { get; set; }
        public string PostalCode { get; set; }
        public bool ExternLogin { get; set; }
        public ApplicantViewModel model { get; set; }
        public List<Message> Messages { get; set; }
        public Country Country;
    }
}