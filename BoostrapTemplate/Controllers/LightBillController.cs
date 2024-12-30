using BoostrapTemplate.ViewModels.LightBill;
using DATA.Interfaces;
using DATA.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BoostrapTemplate.Controllers
{
    public class LightBillController : Controller
    {
        private readonly ILightBill _lightBill;
        private readonly ICustDetails _custDetails;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LightBillController(ILightBill lightBill,ICustDetails custDetails,IWebHostEnvironment webHostEnvironment)
        {
            _lightBill = lightBill;
            _custDetails = custDetails;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            try
            {
                DateTime d = DateTime.Now;
                var data = _lightBill.getAll();
                if (data != null)
                {
                    var final = data.Select(x => new LightBill_Create_ViewModel
                    {
                        Id = x.Id,
                        CustName=x.CustDetails.CustName,
                        BillNo=x.BillNo,
                        Billdate = x.Billdate,
                        BillPath = x.BillPath
                    });

                    var model = new LightBill_List_ViewModel()
                    {
                        lstBilldetails = final
                    };
                    return View(model);
                }
                return View();

            }
            catch (Exception ex)
            {

                throw;
            }
          
        }
        [HttpGet]
        public IActionResult create()
        {
            ViewBag.Custdetails = new SelectList(_custDetails.getAll(), "CustID", "CustName");
            return View();
        }
        [HttpPost]
        public IActionResult create(LightBill_Create_ViewModel model)
        {
            if (ModelState.IsValid)
            {
                string filename=PdfFileName(model.PDFupload);
                LightBill obj = new LightBill() {
                    Billdate = model.Billdate,
                    BillPath= filename,
                    BillNo=model.BillNo,
                    DueDate=model.DueDate,
                    DueAmount=model.DueAmount,
                    PrevUnit=model.PrevUnit,
                    CurrUnit=model.CurrUnit,
                    TotalUnit=model.TotalUnit,
                    CustId=model.CustId
                };
                _lightBill.Add(obj);
                TempData["message"] = "Record save successfully";
                return RedirectToAction("Index");
            }

            return View();
        }
        public string PdfFileName(IFormFile formFile)
        {
            string uniueName = string.Empty;
            if (formFile!=null)
            {
                string uploadfolder = Path.Combine(_webHostEnvironment.WebRootPath, "PDF/");
                uniueName = Guid.NewGuid().ToString()+"_"+formFile.FileName;
                string filePath = Path.Combine(uploadfolder, uniueName);
                using (var filestream=new FileStream(filePath,FileMode.Create))
                {
                    formFile.CopyTo(filestream);
                }
            }
            return uniueName;
        }
        [HttpGet]
        public IActionResult edit(int id)
        {
            if (id!=0)
            {
                ViewBag.Custdetails = new SelectList(_custDetails.getAll(), "CustID", "CustName");
                var data = _lightBill.getDetailsByID(id);
                if (data!=null)
                {
                    LightBill_Edit_ViewModel model = new LightBill_Edit_ViewModel() { 
                    Id=data.Id,
                    CustId=data.CustId,
                    Billdate=data.Billdate,
                    BillNo=data.BillNo,
                    DueDate=data.DueDate,
                    DueAmount=data.DueAmount,
                    PrevUnit=data.PrevUnit,
                    CurrUnit=data.CurrUnit,
                    TotalUnit=data.TotalUnit
                    };
                    return View(model);
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult edit(LightBill_Edit_ViewModel model)
        {
            if (ModelState.IsValid)
            {
                var data = _lightBill.getDetailsByID(model.Id);
                if (data!=null)
                {
                    string uniqueFileName = "";
                    if (model.PDFupload!=null)
                    {
                        string pdfPath = Path.Combine(_webHostEnvironment.WebRootPath, "PDF/", data.BillPath);
                        if (System.IO.File.Exists(pdfPath))
                        {
                            System.IO.File.Delete(pdfPath);
                        }
                        uniqueFileName = PdfFileName(model.PDFupload);
                        data.BillPath = uniqueFileName;
                    }
                    else
                    {
                        data.BillPath = data.BillPath;
                    }

                    data.Billdate = model.Billdate;                    
                    data.BillNo = model.BillNo;
                    data.DueDate = model.DueDate;
                    data.DueAmount = model.DueAmount;
                    data.PrevUnit = model.PrevUnit;
                    data.CurrUnit = model.CurrUnit;
                    data.TotalUnit = model.TotalUnit;
                    data.CustId = data.CustId;
                   
                    _lightBill.Update(data);
                    TempData["message"] = "Record update successfully";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        public IActionResult Delete(int Id)
        {
            if (Id!=0)
            {
                var data = _lightBill.getDetailsByID(Id);
                if (data!=null)
                {
                    string filepath = Path.Combine(_webHostEnvironment.WebRootPath, "PDF/", data.BillPath);
                    if (System.IO.File.Exists(filepath))
                    {
                        System.IO.File.Delete(filepath);
                    }

                    _lightBill.Delete(data); 
                    TempData["message"] = "Record Deleted successfully";
                    return RedirectToAction("index");
                }
            }
           return NotFound();
        }

        public IActionResult Report()
        {
            return View();
        }
    }
}
