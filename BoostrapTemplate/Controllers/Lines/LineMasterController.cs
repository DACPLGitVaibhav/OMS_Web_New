using DATA;
using DATA.Interfaces;
using DATA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMS_Web.ViewModels.Lines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMS_Web.Controllers.Lines
{
    [Authorize(Roles = "Admin")]
    public class LineMasterController : Controller
    {
        private readonly ILineMaster _linemaster;
        public readonly ContextClass _context;


        public LineMasterController(ILineMaster lineMaster, ContextClass context)
        {
            _linemaster = lineMaster;
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult LineOrder()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                ViewData["Heading"] = "Line Order Management";
                ViewBag.LineList = _linemaster.GetAllLines();
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }
            return View();
        }

        public IActionResult GetAllLine()
        {
            return Json(_linemaster.GetAllLines());
        }
        public IActionResult GetLine(LineMaster lineMaster)
        {
            var Linedata = _linemaster.GetLinebyId(lineMaster);

            return Json(Linedata);
        }
        public IActionResult UpdateLOT(LOTViewModel lOTViewModel)
        {
            try
            {
                _context.Database.ExecuteSqlRaw($"Sp_UpdateLOT {lOTViewModel.LineId },{lOTViewModel.Isactive},{lOTViewModel.LOT}");

                return Json(new { status = "LineUpdated" });
            }
            catch (Exception ex)
            {
                var result = new { status = "Error" };
                return Json(result);
                throw;
            }
        }
        [AllowAnonymous]
        public IActionResult Baretail_Log()
        {
            if (HttpContext.Session.GetString("Username") != null)
            {
                try
                {
                    ViewData["Heading"] = "Baretail Log";
                    //ViewBag.LineList = _linemaster.GetAllLines();
                    var lineViewModels = (from bl in _context.tbl_Baretail_Log
                                          join lm in _context.linemasters on bl.LineID equals lm.LineId
                                          join ord in _context.Orders on bl.SeqNo equals ord.SeqNo
                                          select new LOTViewModel
                                          {
                                              ID = bl.ID,
                                              TimeStamp = bl.TimeStamp,
                                              SeqNo = bl.SeqNo,
                                              StatusID = bl.StatusID,
                                              LineName = lm.LineName,
                                              Itemid = ord.ItemId,
                                              Biwno = ord.BiwNo
                                          }).OrderByDescending(x => x.TimeStamp).Take(200).ToList();
                    return View(lineViewModels);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }
            return View();
        }
        public JsonResult ReloadBaretail_Log()
        {
            ViewData["Heading"] = "Baretail Log";
            //ViewBag.LineList = _linemaster.GetAllLines();
            var lineViewModels = (from bl in _context.tbl_Baretail_Log
                                  join lm in _context.linemasters on bl.LineID equals lm.LineId
                                  join ord in _context.Orders on bl.SeqNo equals ord.SeqNo
                                  select new LOTViewModel
                                  {
                                      ID = bl.ID,
                                      TimeStamp = bl.TimeStamp,
                                      SeqNo = bl.SeqNo,
                                      StatusID = bl.StatusID,
                                      LineName = lm.LineName,
                                      Itemid = ord.ItemId,
                                      Biwno = ord.BiwNo
                                  }).OrderByDescending(x => x.TimeStamp).Take(200).ToList();
            return Json(lineViewModels.Distinct());
        }
        public IActionResult Get_Baretail_Log_ByLineId(LOTViewModel lOTViewModel)
        {
            var Baretail_Log_Date = (from bl in _context.tbl_Baretail_Log
                                     join lm in _context.linemasters on bl.LineID equals lm.LineId
                                     join ord in _context.Orders on bl.SeqNo equals ord.SeqNo
                                     where lm.LineId == lOTViewModel.LineId
                                     orderby bl.SeqNo descending, bl.TimeStamp descending
                                     select new LOTViewModel
                                     {
                                         ID = bl.ID,
                                         TimeStamp = bl.TimeStamp,
                                         SeqNo = bl.SeqNo,
                                         StatusID = bl.StatusID,
                                         LineName = lm.LineName,
                                         LineId = lm.LineId,
                                         Itemid = ord.ItemId,
                                         Biwno = ord.BiwNo
                                     }).Take(200).ToList();
            //Baretail_Log_Date = Baretail_Log_Date.Distinct().ToList();
            return Json(Baretail_Log_Date);
        }
        //Vitthal  30/12/2024  

        public IActionResult GetLines()
        {
            var lines = _linemaster.GetAllLineS();

            return Json(lines);
        }

        public IActionResult UpdateStatus(LOTViewModel lOTViewModel)
        {
            try
            {
                _context.Database.ExecuteSqlRaw($"Sp_UpdateLOT {lOTViewModel.LineId},{lOTViewModel.Isactive}");

                return Json(new { status = "LineUpdated" });
            }
            catch (Exception ex)
            {
                var result = new { status = "Error" };
                return Json(result);
                throw;
            }
        }
    }
}
