using DATA;
using DATA.Interfaces;
using DATA.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Services.Services
{
   public class OrdersServices :IOrders
    {

        private readonly IConfiguration configuration;
        private readonly ContextClass _context;
        public OrdersServices(IConfiguration configuration, ContextClass context)
        {
            this.configuration = configuration;
            this._context = context;
        }

        public List<PreProductionDetails> GetPreOrders()
        {
            var obj = _context.PreOrders.OrderBy(x => x.PPSeqNo).Where(x=>x.IsProduction==false && (x.Status == 0 || x.Status == 1 || x.Status == 100)).ToList();
            return obj;
        }
        public List<PreProductionDetails> GetDeletedOrders()
        {
            var obj = _context.PreOrders.OrderBy(x => x.PPSeqNo).Where(x => x.IsProduction == false && x.Status == 2).ToList();
            return obj;
        }

        #region V_Code Add
        public List<VariantCode> GetVariantCodes()
        {
            var V_list = _context.variantcode.Distinct().ToList();
            V_list.Insert(0, new VariantCode()
            {
                Erp_Vcode = "--Select All--"
            });
            return V_list;
        }
        #endregion


        private string Sqlconnection()
        {
            return configuration.GetConnectionString("DbConnection").ToString();
        }


        //public OrdersServices GetData()
        //{
        //    try
        //    {


        //        var query = (
        //            from pd in _context.productions
        //            join lm in _context.linemasters on pd.LineID equals lm.LineId
        //            join eod in _context.Orders on pd.ErpSeqNo equals eod.SeqNo
        //            select new
        //            {
        //                pd.ErpSeqNo,
        //                eod.ItemId,
        //                eod.BiwNo,
        //                eod.Vcode,
        //                pd.Status,
        //                lm.LineName
        //            }
        //        );

        //        var groupedQuery = query
        //            .GroupBy(x => new { x.ErpSeqNo, x.ItemId, x.BiwNo, x.Vcode, x.LineName })
        //            .Select(g => new
        //            {
        //                g.Key.ErpSeqNo,
        //                g.Key.ItemId,
        //                g.Key.BiwNo,
        //                g.Key.Vcode,
        //                LineName = g.Key.LineName,
        //                MaxStatus = g.Max(x => x.Status)
        //            })
        //            .ToList();
        //        //IEnumerable<OrdersViewModel> pivotTable = new IEnumerable<OrdersViewModel>();
        //        var pivotTable = groupedQuery
        //            .GroupBy(x => new { x.ErpSeqNo, x.ItemId, x.BiwNo, x.Vcode })
        //            .Select(g => new OrdersViewModel()
        //            {

        //                ErpSeqNo = g.Key.ErpSeqNo,
        //                ItemId = g.Key.ItemId,
        //                BiwNo = g.Key.BiwNo,
        //                Vcode = g.Key.Vcode,
        //                FF = g.FirstOrDefault(x => x.LineName == "FF")?.MaxStatus ?? 0,
        //                FE = g.FirstOrDefault(x => x.LineName == "FE")?.MaxStatus ?? 0,
        //                RF = g.FirstOrDefault(x => x.LineName == "RF")?.MaxStatus ?? 0,
        //                BSRH = g.FirstOrDefault(x => x.LineName == "BSRH")?.MaxStatus ?? 0,
        //                BSLH = g.FirstOrDefault(x => x.LineName == "BSLH")?.MaxStatus ?? 0
        //            })
        //            .OrderByDescending(x => x.ErpSeqNo)
        //            .ToList();
        //        return (pivotTable);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

     

    }
}
