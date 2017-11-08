using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppFactory.MVC.ViewModels
{
    public class StepThreeViewModel
    {
        public long ApplicantID { get; set; }

        [Required(ErrorMessage = "Company is Required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Position is Required")]
        public string Position { get; set; }

        public string Responsibility { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }
}