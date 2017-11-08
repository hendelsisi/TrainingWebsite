using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppFactory.MVC.ViewModels
{
    public class StepFiveViewModel
    {
        public long ApplicantID { get; set; }
        public int LanguageID { get; set; }
        public int SpokenLevel { get; set; }
        public int WrittenLevel { get; set; }
    }
}