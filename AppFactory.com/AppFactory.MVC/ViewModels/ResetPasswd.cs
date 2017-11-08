using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppFactory.MVC.ViewModels
{
    public class ResetPasswd
    {

        public long UserID { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "Password must be Minimum 8 characters at least 1 Uppercase Alphabet, 1 Lowercase Alphabet and 1 Number")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage ="Confirm Password is Required")]
        [Compare("NewPassword" , ErrorMessage = "Password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

    }
}