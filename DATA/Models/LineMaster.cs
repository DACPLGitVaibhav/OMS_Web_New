using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DATA.Models
{
    [Table("tbl_LineMaster")]
    public class LineMaster
    {
        public int Id { get; set; } 
        public int LineId { get; set; }
        public string LineName { get; set; }
        public string Description { get; set; }
        public DateTime? Createdon { get; set; }
        public bool Isactive { get; set; }
        public Nullable<DateTime> Updatedon { get; set; }
        [ForeignKey("LineId")]
        public LOT_details lOT_Details { get; set; }
        //[ForeignKey("LineID")]
        //public Baretail_Log baretail_Log { get; set; }
        //public IEnumerable<LineMaster> lstline { get; set; }
    }
}
