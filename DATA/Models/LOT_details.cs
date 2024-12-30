using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    [Table("tbl_LOT_details")]
    public class LOT_details
    {
       [Key]
        public int ID { get; set; }
        public int LineId { get; set; }
        public string LOT { get; set; }
        public string MesVCode { get; set; } 
    }
}
