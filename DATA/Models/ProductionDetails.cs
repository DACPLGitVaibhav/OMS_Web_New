using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    [Table("tbl_ProductionDetails")]
    public class ProductionDetails
    {
       [Key]
        public int Id { get; set; }
        public int LineID { get; set; }
        public int Status { get; set; }
        public int ErpSeqNo { get; set; } 
    }
}
