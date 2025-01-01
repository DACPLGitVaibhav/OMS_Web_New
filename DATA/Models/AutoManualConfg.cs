using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    
    [Table("tbl_AutoManualConfg")]
   
    public class AutoManualConfg
    {
        public int PPSeqNo { get; set; }
        public bool IsAutoMode { get; set; }
    }
}
