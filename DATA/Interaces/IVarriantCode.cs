using DATA.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DATA.Interaces
{
    public interface IVarriantCode
    {
        VariantCode getbyid(int id);
        VariantCode getbyName(string name);
        List<VariantCode> Getall();
        void Add(VariantCode variantCode);
        void Edit(VariantCode variantCode);
        void Delete(VariantCode variantCode);
    }
}
