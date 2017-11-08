using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppFactory.MVC.ViewModels
{
    public class StepOneViewModel
    {
        public long UserID { get; set; }

        [Required(ErrorMessage = "First Name is Required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "First Name can't be less than 3 letters or more than 50")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Last Name can't be less than 3 letters or more than 50")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "Date of birth is Required")]
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        [Required(ErrorMessage = "Telephone is Required")]
        public string TelephoneNumber { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{5})$", ErrorMessage = "Entered phone format is not valid.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Country is Required")]
        public int CountryID { get; set; }

        [Required(ErrorMessage = "Postal Code is Required")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }
        public string City { get; set; }
        public string Nationality { get; set; }

        [Required(ErrorMessage = "Resume is Required")]
        public string Resume { get; set; }

        [Required(ErrorMessage = "Cover Letter is Required")]
        public string CoverLetter { get; set; }
    }
}