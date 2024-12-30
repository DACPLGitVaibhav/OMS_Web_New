using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    [Table("tbl_Baretail_Log")]
    public class Baretail_Log
    {   [Key]
        public long ID { get; set; }
        public DateTime TimeStamp { get; set; }
        public int LineID { get; set; }
        public int SeqNo { get; set; }
        public int StatusID { get; set; }
       

    }
}
