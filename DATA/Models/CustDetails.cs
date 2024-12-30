using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    [Table("tbl_CustDetails")]
    public class CustDetails
    {
        [Key]
        public int CustID { get; set; }
        public string CustNo { get; set; }
        public string CustName { get; set; }
        public string Address { get; set; }
        public string MeterNo { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
    }
}
