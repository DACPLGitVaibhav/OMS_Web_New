using DATA.Interaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OMS_Template.ViewModels.OPCUA;
using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OMS_Template.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OPCUAController : Controller
    {
        public readonly ILOTDetials _lOTDetials;
        private readonly OpcUaClientService _opcUaClientService;
       
        public OPCUAController(OpcUaClientService opcUaClientService,ILOTDetials lOTDetials)
        {
            _opcUaClientService = opcUaClientService;
            _lOTDetials = lOTDetials;
        }

        public async Task<IActionResult> Index()
        {
            await _opcUaClientService.ConnectAsync();
            ViewData["Heading"] = "Connection Status";
            return View();
        }

        public  async Task<JsonResult> Status()
        {
            OPCUADetails _oPCUADetails = new OPCUADetails();

            //Getting OMS Live LOT Data
            var Lotdetails = _lOTDetials.GetAll();
            if (Lotdetails != null)
            {
                _oPCUADetails.OMS_FF = Lotdetails.Where(x => x.LineId == 1).Select(c => c.LOT).FirstOrDefault();
                _oPCUADetails.OMS_FE = Lotdetails.Where(x => x.LineId == 2).Select(c => c.LOT).FirstOrDefault();
                _oPCUADetails.OMS_RF = Lotdetails.Where(x => x.LineId == 3).Select(c => c.LOT).FirstOrDefault();
                _oPCUADetails.OMS_BSRH = Lotdetails.Where(x => x.LineId == 4).Select(c => c.LOT).FirstOrDefault();
                _oPCUADetails.OMS_BSLH = Lotdetails.Where(x => x.LineId == 5).Select(c => c.LOT).FirstOrDefault();
            }

            //Getting Master PLC Live LOT Data
            var v =await _opcUaClientService.ReadNode();
            if (v != null)
            {
                _oPCUADetails.Isconnect = v.Isconnect;
                _oPCUADetails.MPLC_FF = v.MPLC_FF;
                _oPCUADetails.MPLC_FE = v.MPLC_FE;
                _oPCUADetails.MPLC_RF = v.MPLC_RF;
                _oPCUADetails.MPLC_BSRH = v.MPLC_BSRH;
                _oPCUADetails.MPLC_BSLH = v.MPLC_BSLH;

                if (_oPCUADetails.Isconnect==false)
                {
                    _oPCUADetails.OMS_FF = v.OMS_FF;
                    _oPCUADetails.OMS_FE = v.OMS_FE;
                    _oPCUADetails.OMS_RF = v.OMS_RF;
                    _oPCUADetails.OMS_BSRH = v.OMS_BSRH;
                    _oPCUADetails.OMS_BSLH = v.OMS_BSLH;
                }              

                return Json(_oPCUADetails);
            }
            else
            {
                _oPCUADetails.Isconnect = false;
                _oPCUADetails.MPLC_FF = "0";
                _oPCUADetails.MPLC_FE = "0";
                _oPCUADetails.MPLC_RF = "0";
                _oPCUADetails.MPLC_BSRH = "0";
                _oPCUADetails.MPLC_BSLH = "0";

                _oPCUADetails.OMS_FF = "0";
                _oPCUADetails.OMS_FE = "0";
                _oPCUADetails.OMS_RF = "0";
                _oPCUADetails.OMS_BSRH = "0";
                _oPCUADetails.OMS_BSLH = "0";

                return Json("Error");
            }                        
        }

        public async Task<JsonResult> TestOPCSession()
        {
            return Json(await _opcUaClientService.TestOPCSession());
        }

        public async Task<IActionResult> LinemgmtDetails(int ID)
        {
          
            if (ID == 1)
            {
                var MGMTDetails = await _opcUaClientService.ReadNodeLinemgmtFF();
                return Json(MGMTDetails);
            }
            else if (ID == 2)
            {
                var MGMTDetails = await _opcUaClientService.ReadNodeLinemgmtFE();
                return Json(MGMTDetails);
            }
            else if (ID == 3)
            {
                var MGMTDetails = await _opcUaClientService.ReadNodeLinemgmtRF();
                return Json(MGMTDetails);
            }
            else if (ID == 4)
            {
                var MGMTDetails = await _opcUaClientService.ReadNodeLinemgmtBSRH();
                return Json(MGMTDetails);
            }
            else if (ID == 5)
            {
                var MGMTDetails = await _opcUaClientService.ReadNodeLinemgmtBSLH();
                return Json(MGMTDetails);
            }
            return Json("Error");
        }
    }
}
