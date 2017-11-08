using AppFactory.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppFactory.MVC.ViewModels
{
    public class ApplicantDetailsViewModel
    {
        //User Table
        public long AdminID { get; set; }
        public long UserID { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }

        //Applicant Table
        public long ApplicantID { get; set; }
        public Applicant Applicant { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Nationality { get; set; }
        public string Resume { get; set; }
        public string CoverLetter { get; set; }
        public int WorkflowStatus { get; set; }


        //PersonalDetail Table
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public System.DateTime DateOfBirth { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string PostalCode { get; set; }

        public Country Country { get; set; }
        public Education Edu { get; set; }

        public WorkExperience Work { get; set; }

        public List<Applicant_Skill> Skill { get; set; }
        public List<Applicant_Language> Languages { get; set; }
        public Message message { get; set; }
    }
}