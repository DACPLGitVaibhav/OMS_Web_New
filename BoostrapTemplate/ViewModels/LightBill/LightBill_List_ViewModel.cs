using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoostrapTemplate.ViewModels.LightBill
{
    public class LightBill_List_ViewModel
    {
        public IEnumerable<LightBill_Create_ViewModel> lstBilldetails { get; set; }
    }
}
