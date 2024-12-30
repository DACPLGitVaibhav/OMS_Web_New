using DATA.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;

namespace BoostrapTemplate.Controllers
{
    public class AnalyticController : Controller
    {
        private readonly ILightBill _lightBill;

        public AnalyticController(ILightBill lightBill)
        {
            _lightBill = lightBill;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ApexChart()
        {
           // var data = _lightBill.getAll().Select(x => new { month = x.Billdate.ToString("MMM"), x.TotalUnit });
         
            return View();
        }

       public JsonResult jsonBasicLine()
        {
      
            var data = _lightBill.getAll().Select(x=>new { month = x.Billdate.ToString("MMM"),x.TotalUnit });
            return Json(data);
        }

        public JsonResult jsonLinewithDataLabels()
        {
            var data = _lightBill.getAll().Select(x => new { month = x.Billdate.ToString("MMM"), x.CurrUnit,x.PrevUnit });
            return Json(data);
        }
    }
}
