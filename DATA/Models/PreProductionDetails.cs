using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    [Table("tbl_PreProductionDetails")]
    public class PreProductionDetails
    {
        [Key]
        public int PPSeqNo { get; set; } 
        public string ItemId { get; set; }
        public string BiwNo { get; set; }
        public string Vcode { get; set; }
        public string ModelCode { get; set; }
        public DateTime DateIimport { get; set; }
        public string FileName { get; set; }
        public bool IsProduction { get; set; }
        public Nullable<DateTime> TImestamp { get; set; }
      
    }
}
