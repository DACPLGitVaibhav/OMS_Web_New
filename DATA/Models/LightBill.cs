using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    [Table("tbl_BillDetails")]
   public class LightBill
    {
        [Key]
        public int Id { get; set; }
        public DateTime Billdate { get; set; }
        public string BillPath { get; set; }
        public string BillNo { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal DueAmount { get; set; }
        public int PrevUnit { get; set; }
        public int CurrUnit { get; set; }
        public int TotalUnit { get; set; }
        public int CustId { get; set; }
        [ForeignKey("CustId")]
        public CustDetails CustDetails { get; set; }
    }
}
