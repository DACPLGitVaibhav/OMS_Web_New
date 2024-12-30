using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    [Table("tbl_ErpOrderDetails")]
    public class ErpOrderDetails
    {
        [Key]
        public int SeqNo { get; set; }
        public string ItemId { get; set; }
        public string BiwNo { get; set; }
        public string Vcode { get; set; }
        public string ModelCode { get; set; }
        public int PPSeqno { get; set; } 
        public DateTime DateIimport { get; set; }  
    }
}
