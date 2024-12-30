using DATA;
using DATA.Interfaces;
using DATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services
{
 public   class CustDetailsServices: ICustDetails
    {
        private readonly ContextClass _context;

        public CustDetailsServices(ContextClass contextClass)
        {
            _context = contextClass;
        }

        public IEnumerable<CustDetails> getAll()
        {
            return _context.CustDetails.ToList();
        }
    }
}
