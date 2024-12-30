using DATA;
using DATA.Interaces;
using DATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services
{
    public class LOTDetailsServices : ILOTDetials
    {
        private readonly ContextClass _context;

        public LOTDetailsServices(ContextClass context)
        {
            _context = context;
        }

        public List<LOT_details> GetAll()
        {
            var v = new List<LOT_details>();
            v = _context.lOT_Details.ToList();
            return v;
        }
    }
}
