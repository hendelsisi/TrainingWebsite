using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AppFactory.MVC.ViewModels
{
    public class RegisterationViewModel
    {
        [Required(ErrorMessage = "First Name is Required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "First Name can't be less than 3 letters or more than 50")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Last Name can't be less than 3 letters or more than 50")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address , ex : john@hotmail.com")]
        [Remote("IsValidEmail", "User", ErrorMessage = "This email is allready in use")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "Password must be Minimum 8 characters at least 1 Uppercase Alphabet, 1 Lowercase Alphabet and 1 Number")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is Required")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        public string Avatar { get; set; }

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{5})$", ErrorMessage = "Entered phone format is not valid.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Country is Required")]
        public int CountryID { get; set; }

        public bool ExternLogin { get; set; }

        [Required(ErrorMessage = "Postal Code is Required")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Postal Code")]
        public string PostalCode { get; set; }
    }
}