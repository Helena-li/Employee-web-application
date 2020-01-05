using EmployeeWebApplication.Utills;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeWebApplication.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [ValidEmailDomain(allowDomain: "aaa.com", ErrorMessage = "Email Domain must be aaa.com")]
        [Remote(action: "IsEmailInUse", controller: "account")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
            ErrorMessage = "Password is not consist with the Confirmation password")]
        public string ConfirmPassword { get; set; }

        public string City { get; set; }
    }
}
