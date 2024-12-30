using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OMS_Template.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoostrapTemplate.Controllers
{
   
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Index Page";
            return View();
        }
        public IActionResult Index2()
        {
            ViewData["Title"] = "Index2 Page";
            return View();
        }
        public IActionResult Create()
        {
            ViewData["Title"] = "Create Page";
            return View();
        }
        public IActionResult Edit()
        {
            ViewData["Title"] = "Edit Page";
            return View();
        }
        public IActionResult Report()
        {
            ViewData["Title"] = "Report Page";
            return View();
        }
        
        [Route("Home/ErrorPage")]
        public IActionResult ErrorPage()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            return View(new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                ErrorMessage = exception.Message
            }); 
        }
    }
}
