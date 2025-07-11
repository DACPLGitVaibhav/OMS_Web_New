using DATA;
using DATA.Interaces;
using DATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services
{
    public class VarriantCodeService : IVarriantCode
    {
        private readonly ContextClass _contextClass;

        public VarriantCodeService(ContextClass contextClass)
        {
            _contextClass = contextClass;
        }
        public void Add(VariantCode variantCode)
        {
            _contextClass.variantcode.Add(variantCode);
            _contextClass.SaveChanges();
        }

        public void Delete(VariantCode variantCode)
        {
            try
            {
                var data=_contextClass.variantcode.Find(variantCode.Id);
                if (data != null)
                {
                    _contextClass.variantcode.Remove(data);
                    _contextClass.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Edit(VariantCode variantCode)
        {
            _contextClass.variantcode.Update(variantCode);
            _contextClass.SaveChanges();
        }

        public List<VariantCode> Getall()
        {
            return _contextClass.variantcode.ToList();
        }

        public VariantCode getbyid(int id)
        {
            return _contextClass.variantcode.Where(c=>c.Id==id).FirstOrDefault();
        }

        public VariantCode getbyName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
