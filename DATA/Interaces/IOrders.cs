using DATA.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace DATA.Interfaces
{
    public interface IOrders
    {
        //OrdersServices GetData();
        List<PreProductionDetails> GetPreOrders();

        List<VariantCode> GetVariantCodes();
    }
}
