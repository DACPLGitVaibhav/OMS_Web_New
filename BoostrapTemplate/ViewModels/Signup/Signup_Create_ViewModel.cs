using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoostrapTemplate.ViewModels.Signup
{
    public class Signup_Create_ViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter User Name")]
        [Display(Name = "User Name")]
       // [Remote(action: "UserAlreadyExist", controller: "Account")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter User Email")]
        [Display(Name = "User Email")]
        //[RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Enter Valid email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter User Mobile(+91)")]
        [Display(Name = "User Mobile")]
        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$", ErrorMessage = "Enter valid Mobile no.(+91)")]

        public string Mobile { get; set; }

        [Required(ErrorMessage = "Enter User Password")]

        public string Password { get; set; }

        [Required(ErrorMessage = "Enter User Confirm Password")]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Confirm Password Can't match.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Active")]
        public bool Isactive { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Enter Date")]
        public DateTime CreatedDate { get; set; }
        [Required(ErrorMessage = "Enter Time")]
        public TimeSpan Createdtime { get; set; }

        public IFormFile ImagePath { get; set; }
    }
}
