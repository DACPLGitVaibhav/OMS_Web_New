using BoostrapTemplate.ViewModels.Signup;
using DATA.Interaces;
using DATA.Interfaces;
using DATA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OMS_Template.ViewModels.VarraintCode;
using System;
using System.IO;
using System.Linq;

namespace OMS_Template.Controllers.VarriantCode
{
    public class VarraintCodeController : Controller
    {
        private readonly IVarriantCode _varriantCode;

        public VarraintCodeController(IVarriantCode varriantCode)
        {
            _varriantCode = varriantCode;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "Variant Code List";

            var data = _varriantCode.Getall();
            if (data != null)
            {
                var final = data.Select(x => new VarraintCode_Add_ViewModel()
                {
                    Id = x.Id,
                    Erp_Vcode = x.Erp_Vcode,
                    Description = x.Description,
                    Mes_Vcode = x.Mes_Vcode,
                });

                var model = new VarraintCode_List_ViewModel
                {
                    VcList = final
                };
                return View(model);
            }
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VarraintCode_Add_ViewModel modal)
        {
            if (ModelState.IsValid)
            {
                var obj = new VariantCode()
                {
                    Erp_Vcode = modal.Erp_Vcode,
                    Description = modal.Description,
                    Mes_Vcode = modal.Mes_Vcode,
                    CreatedDate = DateTime.Now,
                    
                };
                _varriantCode.Add(obj);
                TempData["message"] = "Record Save successfully";
                return RedirectToAction("index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Enter Valid detials");
            }
            return View(modal);
        }

        public IActionResult Edit(int id)
        {
            var data = _varriantCode.getbyid(id);


            if (data != null)
            {
                try
                {
                    var model = new VarraintCode_Edit_ViewModel()
                    {
                        Id = data.Id,
                        Erp_Vcode = data.Erp_Vcode,
                        Description= data.Description,
                        Mes_Vcode = data.Mes_Vcode
                    };
                    return View(model);
                }
                catch (Exception ex)
                {
                    TempData["message"] = "Something went wrong, " + ex.InnerException.Message;
                    return RedirectToAction("index");
                }
            }
            return RedirectToAction("index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(VarraintCode_Edit_ViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = _varriantCode.getbyid(model.Id);
                   
                    data.Erp_Vcode = model.Erp_Vcode;
                    data.Description = model.Description;
                    data.Mes_Vcode = model.Mes_Vcode;
                    data.UpdatedDate = DateTime.Now;

                    _varriantCode.Edit(data);

                    TempData["message"] = "Record Update successfully";
                    return RedirectToAction("index");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Something went wrong, " + ex.InnerException.Message;
                return RedirectToAction("index");
                //ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }
    }
}
