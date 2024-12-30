using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OMS_Web.ViewModels.Lines
{
    public class LOTViewModel
    {
        [Key]
        public int LineId { get; set; }
        public string LineName { get; set; }
        public string Description { get; set; }
        public DateTime? Createdon { get; set; }
        public bool Isactive { get; set; }
        public Nullable<DateTime> Updatedon { get; set; }
        public string LOT { get; set; }
        public string MesVCode { get; set; }

        public long ID { get; set; }
        public DateTime TimeStamp { get; set; }       
        public int SeqNo { get; set; }
        [Display(Name ="Status")]
        public int StatusID { get; set; }

        public string Itemid { get; set; }

        public string Biwno { get; set; }

    }
}
