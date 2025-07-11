using System.ComponentModel.DataAnnotations;

namespace OMS_Template.ViewModels.VarraintCode
{
    public class VarraintCode_Add_ViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter Erp Vcode")]
        [Display(Name = "Erp Vcode")]
        public string Erp_Vcode { get; set; }
        [Required(ErrorMessage = "Enter Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Enter Mes Vcode")]
        [Display(Name = "Mes Vcode")]
        //[RegularExpression(@"^\d{5}$", ErrorMessage = "The number must be exactly 5 digits.")]
        [Range(0, 99999, ErrorMessage = "Wrong MES Vcode")]
        public int Mes_Vcode { get; set; }
    }
}
