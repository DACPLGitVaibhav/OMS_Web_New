using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoostrapTemplate.ViewModels.Signup
{
    public class Signup_List_ViewModel
    {
      public  IEnumerable<Signup_Create_ViewModel> LstSignup { get; set; }
    }
}
