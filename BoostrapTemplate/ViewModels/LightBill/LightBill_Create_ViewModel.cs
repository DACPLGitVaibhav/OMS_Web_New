using DATA.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoostrapTemplate.ViewModels.LightBill
{
    public class LightBill_Create_ViewModel
    {
        public int Id { get; set; }
        [Display(Name ="Bill Date")]
        [Required]
        public DateTime Billdate { get; set; }
        [Display(Name = "Upload Bill")]
        [Required(ErrorMessage = "Choose PDF bill")]
        public IFormFile PDFupload { get; set; }
        public string BillPath { get; set; }
        [Required(ErrorMessage = "Enter Bill no")]
        [Display(Name ="Bill No")]        
        public string BillNo { get; set; }
        [Display(Name = "Due Date")]
        [Required(ErrorMessage = "Select Due Date")]
        public DateTime? DueDate { get; set; }
        [Display(Name = "Due Amount")]
        [Required(ErrorMessage = "Enter Due Amount")]
        public decimal DueAmount { get; set; }
        [Display(Name = "Previous unit")]
        [Required(ErrorMessage = "Enter Previous unit")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int PrevUnit { get; set; }
        [Display(Name = "Current unit")]
        [Required(ErrorMessage = "Enter Current unit")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int CurrUnit { get; set; }
        [Display(Name = "Enter Total unit")]
        [Required(ErrorMessage = "Enter Total unit")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int TotalUnit { get; set; }
        [Required(ErrorMessage ="Select Customer")]
        [Display(Name ="Customer Name")]
        public int CustId { get; set; }     
        public string CustName { get; set; }     
        public CustDetails CustDetails { get; set; }
    }
}
