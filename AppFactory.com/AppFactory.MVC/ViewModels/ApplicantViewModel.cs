using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppFactory.DAL;

namespace AppFactory.MVC.ViewModels
{
    public class ApplicantViewModel
    {
        public long UserID { get; set; }
        public long ApplicantID { get; set; }
        public int AppStatus { get; set; }
        public List<Application_Applicant> History { get; set; }

        // Values No Application - Incomplete Application - Submitted Application
        
        public int? Step { get; set; }
    }
}