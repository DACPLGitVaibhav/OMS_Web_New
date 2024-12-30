﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoostrapTemplate.ViewModels.Login
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name ="User Name")]
        public string Username { get; set; }
        [Required]
        //[Display(Name = "User Name")]
        public string Password { get; set; }
    }
}
