using DATA;
using DATA.Interfaces;
using DATA.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Services.Services
{
   public class LineServices:ILineMaster
    {
       
        public readonly ContextClass _context;


        public LineServices( ContextClass context)
        {
           
            _context = context;
        }
        public List<LineMaster>  GetAllLines()
        {

            List<LineMaster> linelist = new List<LineMaster>();
            linelist =_context.linemasters.ToList();
            //linelist.Insert(0, new LineMaster()
            //{
            //    LineId = 0,
            //    LineName = "--Select Line--" 
            //});
           return linelist; 
        }

        public LineMaster GetLinebyId(LineMaster lineMaster)
        {
            try
            {
                var line = _context.linemasters.Include(m => m.lOT_Details).Where(m => m.LineId == lineMaster.LineId).FirstOrDefault();
                return line;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<LineMaster> GetAllLineS()
        {
            try
            {
                var lines = _context.linemasters.Include(m => m.lOT_Details).ToList();
                return lines;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
