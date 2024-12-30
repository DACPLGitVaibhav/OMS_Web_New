using DATA;
using DATA.Interfaces;
using DATA.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services
{
   public class LightBillServices:ILightBill
    {
        private readonly ContextClass _context;

        public LightBillServices(ContextClass contextClass)
        {
            _context = contextClass;
        }

        public void Add(LightBill lightBill)
        {
            _context.lightBills.Add(lightBill);
            _context.SaveChanges();
        }

        public void Delete(LightBill lightBill)
        {
            _context.lightBills.Remove(lightBill);
            _context.SaveChanges();
        }

        public List<LightBill> getAll()
        {
            return _context.lightBills.Include(x=>x.CustDetails).ToList();
        }

        public LightBill getDetailsByID(int id)
        {
            return _context.lightBills.Where(x => x.Id == id).FirstOrDefault();
        }

        public void Update(LightBill lightBill)
        {
            _context.lightBills.Update(lightBill);
            _context.SaveChanges();
        }
    }
}
