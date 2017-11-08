using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppFactory.MVC.ViewModels
{
    public class ApplicantListViewModel
    {
        public long AdminID { get; set; }
        public long ApplicantID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
    }
}