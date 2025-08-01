using DATA;
using DATA.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OMS_Web.ViewModels.DataVisulization;
using System;
using System.Collections.Generic;
using System.Linq;
using DATA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using OMS_Template.ViewModels.AutoManual;
using System.Xml.Linq;


namespace OMS_Web.Controllers.DataVisulization
{
    [Authorize(Roles = "Admin")]
    public class DataVisulizationController : Controller
    {

        private readonly ContextClass _context;
        private readonly IOrders _orders;
        public static List<LineStatusMgmtNodes> FF;
        public static List<LineStatusMgmtNodes> FE;
        public static List<LineStatusMgmtNodes> RF;
        public static List<LineStatusMgmtNodes> BSLH;
        public static List<LineStatusMgmtNodes> BSRH;
        public DataVisulizationController(ContextClass context, IOrders orders)
        {
            _context = context;
            _orders = orders;
           
        }

        public IActionResult PreProductionOrders(string erpCode)
        {
            if (HttpContext.Session.GetString("Username") != null)
            {

                var obj = _orders.GetPreOrders();

                if (!string.IsNullOrEmpty(erpCode) && erpCode != "--Select All--")
                {
                    obj = obj.Where(x => x.Vcode == erpCode).ToList();
                }

                var variantCodes = _orders.GetVariantCodes();
                ViewBag.V_list = new SelectList(variantCodes, "Erp_Vcode", "Erp_Vcode");

                var data = _context.autoManualConfgs.ToList();

                //if (data != null)
                //{
                //    if (data[0].IsAutoMode == true)
                //    {
                //        TempData["PPSeqNo"] = data[0].PPSeqNo; //1234567789;

                //    }

                //}
                ViewData["Heading"] = "Pre-Production Orders Plan";


                return View(obj);
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }
        }

        public IActionResult DeletedOrders()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {

                var obj = _orders.GetDeletedOrders();          
                ViewData["Heading"] = "Deleted Orders";


                return View(obj);
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }
        }

        [HttpPost]
        public IActionResult TRFtoProduction([FromBody] List<PreProductionDetails> customDataArray)
        {

            try
            {
                if (User.Identity.Name == "Admin")
                {
                    bool manual = _context.autoManualConfgs.Any(x => x.IsAutoMode == false);
                    if (manual == true)
                    {
                        var seqNos = customDataArray.Select(x => x.BiwNo).ToList();

                        bool isProduction = _context.PreOrders
                            .Where(x => seqNos.Contains(x.BiwNo))
                            .All(x => x.Status == 0 );
                       
                        if (isProduction)
                        {
                            if (customDataArray.Count != 0 || customDataArray != null)
                            {
                                customDataArray = customDataArray.OrderBy(x => x.PPSeqNo).ToList();

                                foreach (var item in customDataArray)
                                {

                                    _context.Database.ExecuteSqlRaw
                                      (
                                       "EXEC SP_InsertErpOrderDetails @ItemId, @BiwNo, @Vcode, @ModelCode, @PPSeqNo",
                                       new SqlParameter("@ItemId", item.ItemId),
                                       new SqlParameter("@BiwNo", item.BiwNo),
                                       new SqlParameter("@Vcode", item.Vcode),
                                       new SqlParameter("@ModelCode", item.ModelCode),
                                       new SqlParameter("@PPSeqNo", item.PPSeqNo)
                                      );

                                }
                                var result = new { status = "DataRecived" };
                                return Json(result);
                            }
                            else
                            {
                                var result = new { status = "error" };
                                return Json(result);

                            }
                        }
                        else
                        {
                            var result = new { status = "Unauthorized", message = "Order Already Hold/Deleted." };
                            return Json(result);

                        }
                    }
                    else
                    {
                        var result = new { status = "error" };
                        return Json(result);

                    }


                }
                else
                {
                    // If user is not in "Admin" role, return unauthorized status code
                    var result = new { status = "Unauthorized", message = "You are not authorized to access this resource." };
                    return Json(result);
                }


            }

            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost]
        public IActionResult IsDeleted([FromBody] List<PreProductionDetails> customDataArray)
        {

            try
            {
                if (User.Identity.Name == "Admin")
                {
                    if (customDataArray.Count != 0 || customDataArray != null)
                    {
                        customDataArray = customDataArray.OrderBy(x => x.PPSeqNo).ToList();
                        var ppSeqNos = customDataArray.Select(x => x.PPSeqNo).ToList();
                        bool isAuto = _context.autoManualConfgs.Any(x => x.IsAutoMode == true);
                        bool isProduction = _context.PreOrders.Where(x => ppSeqNos.Contains(x.PPSeqNo)).Any(x => x.IsProduction == true);
                        bool isDeleted = _context.PreOrders.Where(x => ppSeqNos.Contains(x.PPSeqNo)).Any(x => x.Status == 2);
                        bool isHold = _context.PreOrders.Where(x => ppSeqNos.Contains(x.PPSeqNo)).Any(x => x.Status == 1);

                        if (!isProduction && !isAuto && !isDeleted && !isHold)
                        {
                            foreach (var item in customDataArray)
                            {

                                _context.Database.ExecuteSqlRaw
                                      (
                                       "EXEC SP_IsDeletedOrder @PPSeqNo",

                                       new SqlParameter("@PPSeqNo", item.PPSeqNo)
                                      );

                            }
                            var result = new { status = "DataDeleted" };
                            return Json(result);
                        }
                        else
                        {

                            string message = "";

                            if (isProduction)
                                message += "Item(s) are already in production. Deletion not allowed. ";

                            if (isAuto)
                                message += "Please turn off Auto mode to allow deletion.";

                            if (isDeleted)
                                message += "Order(s) already Deleted.";
                            if (isHold)
                                message += "Please Released Order(s) first.";

                            return Json(new { status = "error", message });
                        }
                       
                    }
                    else
                    {
                        var result = new { status = "error" };
                        return Json(result);

                    }



                }
                else
                {
                    // If user is not in "Admin" role, return unauthorized status code
                    var result = new { status = "Unauthorized", message = "You are not authorized to access this resource." };
                    return Json(result);
                }


            }

            catch (Exception ex)
            {
                var result = new { status = "Unauthorized", message = ex.Message.ToString() };
                return Json(result);
            }

        }

        [HttpPost] 
        public IActionResult Hold([FromBody] List<PreProductionDetails> customDataArray)
        {

            try
            {
                if (User.Identity.Name == "Admin")
                {
                    if (customDataArray.Count != 0 || customDataArray != null)
                    {
                        customDataArray = customDataArray.OrderBy(x => x.PPSeqNo).ToList();
                        var ppSeqNos = customDataArray.Select(x => x.PPSeqNo).ToList();
                        bool isAuto = _context.autoManualConfgs.Any(x => x.IsAutoMode == true);
                        bool isProduction = _context.PreOrders.Where(x => ppSeqNos.Contains(x.PPSeqNo)).Any(x => x.IsProduction == true);
                        bool isDeleted = _context.PreOrders.Where(x => ppSeqNos.Contains(x.PPSeqNo)).Any(x => x.Status == 2);
                        bool isHold = _context.PreOrders.Where(x => ppSeqNos.Contains(x.PPSeqNo)).Any(x => x.Status == 1);
                        if (!isProduction && !isAuto && !isDeleted && !isHold)
                        {
                            foreach (var item in customDataArray)
                            {

                                _context.Database.ExecuteSqlRaw
                                      (
                                       "EXEC SP_HoldOrder @PPSeqNo",

                                       new SqlParameter("@PPSeqNo", item.PPSeqNo)
                                      );

                            }
                            var result = new { status = "DataHold" };
                            return Json(result);
                        }
                        else
                        {

                            string message = "";

                            if (isProduction)
                                message += "Item(s) are already in production. Hold not allowed. ";

                            if (isAuto)
                                message += "Please turn off Auto mode to allow deletion.";

                            if (isHold)
                                message += "Order(s) already Hold .";

                            if (isDeleted)
                            {
                                message += "Order(s) already Deleted .";
                            }

                            return Json(new { status = "error", message });
                        }

                    }
                    else
                    {
                        var result = new { status = "error" };
                        return Json(result);

                    }



                }
                else
                {
                    // If user is not in "Admin" role, return unauthorized status code
                    var result = new { status = "Unauthorized", message = "You are not authorized to access this resource." };
                    return Json(result);
                }


            }

            catch (Exception ex)
            {
                var result = new { status = "Unauthorized", message = ex.Message.ToString() };
                return Json(result);
            }

        }
        [HttpPost] 
        public IActionResult ReleaseHold([FromBody] List<PreProductionDetails> customDataArray)
        {

            try
            {
                if (User.Identity.Name == "Admin")
                {
                    if (customDataArray.Count != 0 || customDataArray != null)
                    {
                        customDataArray = customDataArray.OrderBy(x => x.PPSeqNo).ToList();
                        var ppSeqNos = customDataArray.Select(x => x.PPSeqNo).ToList();
                        bool isAuto = _context.autoManualConfgs.Any(x => x.IsAutoMode == true);
                        bool isProduction = _context.PreOrders.Where(x => ppSeqNos.Contains(x.PPSeqNo)).Any(x => x.IsProduction == true);
                        bool isReleased = _context.PreOrders.Where(x => ppSeqNos.Contains(x.PPSeqNo)).Any(x => x.Status == 0);
                        if (!isProduction && !isAuto && !isReleased)
                        {
                            foreach (var item in customDataArray)
                            {

                                _context.Database.ExecuteSqlRaw
                                      (
                                       "EXEC SP_ReleaseHold @PPSeqNo",

                                       new SqlParameter("@PPSeqNo", item.PPSeqNo)
                                      );

                            }
                            var result = new { status = "Released" };
                            return Json(result);
                        }
                        else
                        {

                            string message = "";

                            if (isProduction)
                                message += "Item(s) are already in production.";

                            if (isAuto)
                                message += "Please turn off Auto mode.";

                            if (isReleased)
                                message += "Order(s) already Released.";

                            return Json(new { status = "error", message });
                        }

                    }
                    else
                    {
                        var result = new { status = "error" };
                        return Json(result);

                    }



                }
                else
                {
                    // If user is not in "Admin" role, return unauthorized status code
                    var result = new { status = "Unauthorized", message = "You are not authorized to access this resource." };
                    return Json(result);
                }


            }

            catch (Exception ex)
            {
                var result = new { status = "Unauthorized", message = ex.Message.ToString() };
                return Json(result);
            }

        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                try
                {
                    var lineorder = (
                          from pd in _context.productions
                          join lm in _context.linemasters on pd.LineID equals lm.LineId
                          join eod in _context.Orders on pd.ErpSeqNo equals eod.SeqNo
                          join vc in _context.variantcode on eod.Vcode equals vc.Erp_Vcode
                          select new
                          {
                              pd.ErpSeqNo,
                              eod.ItemId,
                              eod.BiwNo,
                              vc.Mes_Vcode,
                              // eod.Vcode,
                              pd.Status,
                              lm.LineName,
                              eod.PPSeqno

                          }
                      );

                    var groupedquery = lineorder
                        .GroupBy(x => new { x.ErpSeqNo, x.ItemId, x.BiwNo, x.Mes_Vcode, x.LineName, x.PPSeqno })
                        .Select(g => new
                        {
                            g.Key.ErpSeqNo,
                            g.Key.PPSeqno,
                            g.Key.ItemId,
                            g.Key.BiwNo,
                            g.Key.Mes_Vcode,
                            LineName = g.Key.LineName,
                            MaxStatus = g.Max(x => x.Status)
                        })
                        .ToList();

                    var finalorder = groupedquery
                        .GroupBy(x => new { x.ErpSeqNo, x.PPSeqno, x.ItemId, x.BiwNo, x.Mes_Vcode })
                        .Select(g => new OrdersViewModel()
                        {
                            PPSeqno = g.Key.PPSeqno,
                            ErpSeqNo = g.Key.ErpSeqNo,
                            ItemId = g.Key.ItemId,
                            BiwNo = g.Key.BiwNo,
                            Vcode = g.Key.Mes_Vcode.ToString(),
                            FF = g.FirstOrDefault(x => x.LineName == "FF").MaxStatus,
                            FE = g.FirstOrDefault(x => x.LineName == "FE").MaxStatus,
                            RF = g.FirstOrDefault(x => x.LineName == "RF").MaxStatus,
                            BSRH = g.FirstOrDefault(x => x.LineName == "BSRH").MaxStatus,
                            BSLH = g.FirstOrDefault(x => x.LineName == "BSLH").MaxStatus
                        })
                        .OrderByDescending(x => x.ErpSeqNo).Take(1500).ToList();


                    //var model = new OrdersViewModel()
                    //{
                    //    lstorder = pivotTable
                    //};

                    ViewData["Heading"] = "Order Plan Visualization";
                    return View(finalorder);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }

        }


        public IActionResult TRFToPreProduction([FromBody] string[] ErpSeqArray)
        {
            try
            {
                if (User.Identity.Name == "Admin")
                {
                    if (ErpSeqArray != null)
                    {
                        var seqNos = ErpSeqArray.Select(x => int.Parse(x)).ToList();

                        bool isAllStatusValid = _context.productions
                            .Where(x => seqNos.Contains(x.ErpSeqNo))
                            .All(x => x.Status == 0);
                        bool checkautoStaus = _context.autoManualConfgs.Any(x => x.IsAutoMode ==false);
                        if (checkautoStaus)
                        {


                            if (isAllStatusValid)
                            {
                                foreach (var item in ErpSeqArray)
                                {
                                    _context.Database.ExecuteSqlRaw("EXEC SP_RevertPreProduction @SeqNo", new SqlParameter("@SeqNo", item));

                                }
                                var result = new { status = "Executed" };
                                return Json(result);
                            }
                            else
                            {
                                var result = new { status = "error" };
                                return Json(result);
                            }
                        }
                        else
                        {
                            var result = new { status = "Unauthorized", message="Stop Auto mode" };
                            return Json(result);
                        }
                    }
                    else
                    {
                        var result = new { status = "error" };
                        return Json(result);
                    }

                }
                else
                {
                    // If user is not in "Admin" role, return unauthorized status code
                    var result = new { status = "Unauthorized", message = "You are not authorized to access this resource." };
                    return Json(result);
                }
            }
            catch (Exception)
            {
                var result = new { status = "error" };
                return Json(result);
                //throw;
            }
            // return View();
        }

        [HttpGet]
        public IActionResult GetLineStatus()
        {
            bool status = _context.linemasters.All(x => x.Isactive == false);
            return Json(status);

        }


        public IActionResult GetOrderStatus(int ErpSeqNo)
        {
            bool status = _context.productions.Where(x => x.ErpSeqNo == ErpSeqNo).All(x => x.Status == 0 || x.Status == 100);
            return Json(status);
        }
        [HttpGet]
        public JsonResult ReLoadData()
        {
            try
            {
                var lineorder = (
                      from pd in _context.productions
                      join lm in _context.linemasters on pd.LineID equals lm.LineId
                      join eod in _context.Orders on pd.ErpSeqNo equals eod.SeqNo
                      join vc in _context.variantcode on eod.Vcode equals vc.Erp_Vcode
                      select new
                      {
                          pd.ErpSeqNo,
                          eod.ItemId,
                          eod.BiwNo,
                          vc.Mes_Vcode,
                          // eod.Vcode,
                          pd.Status,
                          lm.LineName,
                          eod.PPSeqno

                      }
                  );

                var groupedquery = lineorder
                    .GroupBy(x => new { x.ErpSeqNo, x.ItemId, x.BiwNo, x.Mes_Vcode, x.LineName, x.PPSeqno })
                    .Select(g => new
                    {
                        g.Key.ErpSeqNo,
                        g.Key.PPSeqno,
                        g.Key.ItemId,
                        g.Key.BiwNo,
                        g.Key.Mes_Vcode,
                        LineName = g.Key.LineName,
                        MaxStatus = g.Max(x => x.Status)
                    })
                    .ToList();

                var finalorder = groupedquery
                    .GroupBy(x => new { x.ErpSeqNo, x.PPSeqno, x.ItemId, x.BiwNo, x.Mes_Vcode })
                    .Select(g => new OrdersViewModel()
                    {
                        PPSeqno = g.Key.PPSeqno,
                        ErpSeqNo = g.Key.ErpSeqNo,
                        ItemId = g.Key.ItemId,
                        BiwNo = g.Key.BiwNo,
                        Vcode = g.Key.Mes_Vcode.ToString(),
                        FF = g.FirstOrDefault(x => x.LineName == "FF").MaxStatus,
                        FE = g.FirstOrDefault(x => x.LineName == "FE").MaxStatus,
                        RF = g.FirstOrDefault(x => x.LineName == "RF").MaxStatus,
                        BSRH = g.FirstOrDefault(x => x.LineName == "BSRH").MaxStatus,
                        BSLH = g.FirstOrDefault(x => x.LineName == "BSLH").MaxStatus
                    })
                    .OrderByDescending(x => x.ErpSeqNo).Take(1500).ToList();

                return Json(finalorder);
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public IActionResult SetBreakPoint(OrdersViewModel ordersViewModel)
        {

            try
            {
                if (User.Identity.Name == "Admin")
                {
                    bool OrderStatus = _context.productions.Where(x => x.ErpSeqNo == ordersViewModel.ErpSeqNo).All(x => x.Status == 0 || x.Status == 100);
                    if (OrderStatus == true)
                    {


                        var erpSeqNoParam = new SqlParameter("@ErpSeqNo", ordersViewModel.ErpSeqNo);

                        if (ordersViewModel.FF == 0 && ordersViewModel.FE == 0 && ordersViewModel.BSRH == 0 && ordersViewModel.BSLH == 0)
                        {

                            _context.Database.ExecuteSqlRaw($"exec SP_SetBreakPoint @ErpSeqNo", erpSeqNoParam);

                            var result = new { status = "SetBreakpoint" };

                            return Json(result);
                        }
                        else if (ordersViewModel.FF == 100 && ordersViewModel.FE == 100 && ordersViewModel.BSRH == 100 && ordersViewModel.BSLH == 100)
                        {
                            _context.Database.ExecuteSqlRaw($"exec SP_ReleseBreakPoint @ErpSeqNo", erpSeqNoParam);
                            var result = new { status = "ReleseBreakPoint" };

                            return Json(result);
                        }
                        else
                        {
                            var result = new { status = "error" };

                            return Json(result);
                        }
                    }
                    else
                    {
                        var result = new { status = "error1" };
                        return Json(result);
                    }
                }
                else
                {
                    // If user is not in "Admin" role, return unauthorized status code
                    var result = new { status = "Unauthorized", message = "You are not authorized to access this resource." };
                    return Json(result);
                }
                //return View();
            }
            catch (Exception ex)
            {
                var result = new { status = "Unauthorized", message = ex.Message.ToString()};
                return Json(result);
            }
            return View();
        }
        //public IActionResult SetAbandoned(OrdersViewModel ordersViewModel)
        //{
        //    try
        //    {
        //        bool OrderStatus = _context.productions.Where(x => x.ErpSeqNo == ordersViewModel.ErpSeqNo).All(x => x.Status == 0 || x.Status == 101);
        //        if (OrderStatus == true)
        //        {
        //            var erpSeqNoParam = new SqlParameter("@ErpSeqNo", ordersViewModel.ErpSeqNo);
        //            if ((ordersViewModel.FF == 101 && ordersViewModel.FE == 101 && ordersViewModel.BSRH == 101 && ordersViewModel.BSLH == 101) ||
        //                (ordersViewModel.FF == 0 && ordersViewModel.FE == 0 && ordersViewModel.BSRH == 0 && ordersViewModel.BSLH == 0)
        //                )
        //            {
        //                _context.Database.ExecuteSqlRaw($"exec SP_SetAbounded @ErpSeqNo", erpSeqNoParam);
        //                var result = new { status = "Set Abandoned" };
        //                return Json(result);
        //            }
        //            else
        //            {
        //                var result = new { status = "error" };
        //                return Json(result);
        //            }
        //        }
        //        else
        //        {
        //            var result = new { status = "error1" };
        //            return Json(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //    return View();
        //}


        public IActionResult SetAbandoned([FromBody] string[] ErpSeqArray)
        {
            try
            {
                if (User.Identity.Name == "Admin")
                {
                    if (ErpSeqArray.Length != 0)
                    {

                        foreach (var item in ErpSeqArray)
                        {

                            bool OrderStatus = _context.productions.Where(x => x.ErpSeqNo == Convert.ToInt32(item)).All(x => x.Status == 0 || x.Status == 101);
                            if (OrderStatus == true)
                            {
                                _context.Database.ExecuteSqlRaw("EXEC SP_SetAbounded @ErpSeqNo", new SqlParameter("@ErpSeqNo", item));

                            }
                            else
                            {
                                var result1 = new { status = "error1" };
                                return Json(result1);
                            }


                        }
                        var result = new { data = "Set Abandoned" };
                        return Json(result);
                    }
                    else
                    {
                        var result = new { status = "error" };
                        return Json(result);
                    }
                }
                else
                {
                    // If user is not in "Admin" role, return unauthorized status code
                    var result = new { status = "Unauthorized", message = "You are not authorized to access this resource." };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var result = new { status = "error" };
                return Json(result);
                //throw;
            }
        }


        public IActionResult SwapOrder([FromBody] string[] ErpSeqArray)
        {
            try
            {
                if (User.Identity.Name == "Admin")
                {
                    if (ErpSeqArray.Length == 2)
                    {
                        for (int i = 0; i < ErpSeqArray.Length; i++)
                        {
                            bool StatusSeqNo1 = _context.productions.Where(x => x.ErpSeqNo == Convert.ToInt32(ErpSeqArray[0])).All(x => x.Status == 0);
                            bool StatusSeqNo2 = _context.productions.Where(x => x.ErpSeqNo == Convert.ToInt32(ErpSeqArray[1])).All(x => x.Status == 0);
                            if (StatusSeqNo1 == true && StatusSeqNo2 == true)
                            {
                                _context.Database.ExecuteSqlRaw("EXEC Sp_SwapOrder @SeqNo1,@SeqNo2",
                                  new SqlParameter("@SeqNo1", ErpSeqArray[0]),
                                  new SqlParameter("@SeqNo2", ErpSeqArray[1])
                                  );
                                var result = new { data = "Executed" };
                                return Json(result);

                            }
                            else
                            {
                                var result1 = new { status = "error1" };
                                return Json(result1);

                            }

                        }


                    }
                    else
                    {
                        var result = new { status = "error" };
                        return Json(result);
                    }
                }
                else
                {
                    // If user is not in "Admin" role, return unauthorized status code
                    var result = new { status = "Unauthorized", message = "You are not authorized to access this resource." };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var result = new { status = "error" };
                return Json(result);
                throw;
            }

            return View();
        }

        public IActionResult Welcome()
        {
            try
            {
                var data = _context.lineStatusMgmtNodes.ToList();
                FF = data.Where(x=>x.ID==1).ToList();
                FE = data.Where(x => x.ID == 2).ToList();
                RF = data.Where(x => x.ID == 3).ToList();
                BSRH = data.Where(x => x.ID == 4).ToList();
                BSLH = data.Where(x => x.ID == 5).ToList();
              
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        [HttpGet]
        public IActionResult AutoManual()
        {
            AutoManualViewModel model = new AutoManualViewModel();

            var data = _context.autoManualConfgs.ToList();
            if (data.Count > 0)
            {
                model.PPSeqNo = data[0].PPSeqNo;
                model.IsAutoMode = data[0].IsAutoMode;
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult AutoManual(AutoManualViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.IsAutoMode == true)
                    {
                        var ppseqno = _context.PreOrders.Where(x => x.PPSeqNo == model.PPSeqNo && x.IsProduction == false).ToList();
                        if (ppseqno.Any(x => x.Status == 0 || x.Status == 100))
                        {
                            //model.PPSeqNo = 0;
                            _context.Database.ExecuteSqlRaw
                                        (
                                         "EXEC SP_AutoManualConfg @PPSeqNo, @IsAutoMode",
                                         new SqlParameter("@PPSeqNo", model.PPSeqNo),
                                         new SqlParameter("@IsAutoMode", model.IsAutoMode)
                                        );
                            TempData["PPseqNotFound"] = $"Auto mode is on.";
                            TempData["PPseqNotFound"] = $"Auto mode is on before PPseqNo: {model.PPSeqNo}.";
                        }
                        //else if(ppseqno.Any(x=>x.Status == 100))
                        //{
                        //    TempData["PPseqNotFound"] = $"PPSeqNo: {model.PPSeqNo} is already in Auto mode.Please release the set point first.";
                        //}
                        else
                        {
                            TempData["PPseqNotFound"] = $"PPSeqNo: {model.PPSeqNo} is either on Hold, Deleted, or not found in Auto mode.";
                        }
                    }
                    else
                    {
                        int i = 0;
                        _context.Database.ExecuteSqlRaw
                                       (
                                        "EXEC SP_AutoManualConfg @PPSeqNo, @IsAutoMode",
                                        new SqlParameter("@PPSeqNo", i),
                                        new SqlParameter("@IsAutoMode", model.IsAutoMode)
                                       );

                        TempData["PPseqNotFound"] = $"Auto mode is off.";
                    }

                    return RedirectToAction("PreProductionOrders", "DataVisulization");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }

        public JsonResult CheckIsAutoMode()
        {
            try
            {
                var result = _context.autoManualConfgs.Where(x => x.IsAutoMode == true).FirstOrDefault();
                if (result != null)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception)
            {
                return Json(false);
            }

        }
        public JsonResult ReloadPreProOrder()
        {
            return Json(_orders.GetPreOrders());
        }
    }
}
