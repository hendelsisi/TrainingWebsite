using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppFactory.MVC.ViewModels
{
    public class StepTwoViewModel
    {
        public long ApplicantID { get; set; }

        [Required(ErrorMessage = "College is Required")]
        public int CollegeID { get; set; }

        [Required(ErrorMessage = "Degree is Required")]
        public string Degree { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Graduation Year is Required")]
        public DateTime GeaduationYear { get; set; }

        [Required(ErrorMessage = "Grade is Required")]
        [Range(minimum: 2, maximum: 4, ErrorMessage = "Enter a valid grade value")]
        public double Grade { get; set; }
    }
}