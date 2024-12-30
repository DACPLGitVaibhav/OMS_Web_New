using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OMS_Web.ViewModels.DataVisulization
{
    public class OrdersViewModel
    {
        [Key]
        public int ErpSeqNo { get; set; }
        public int PPSeqno { get; set; }
        public string ItemId { get; set; }
        public string BiwNo { get; set; }
        public string Vcode { get; set; }
        public int LineID { get; set; } 
        public string LineName { get; set; }
        public int Status { get; set; }

        public int FF { get; set; }
        public int FE { get; set; }
        public int RF { get; set; }
        public int BSLH { get; set; }
        public int BSRH { get; set; }
       
        public IEnumerable<OrdersViewModel> lstorder { get; set; }
        public Nullable<int> PendingOrders { get; set; }



    }
}
