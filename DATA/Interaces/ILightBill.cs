using DATA.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DATA.Interfaces
{
    public interface ILightBill
    {
        List<LightBill> getAll();
        LightBill getDetailsByID(int id);
        void Add(LightBill lightBill);
        void Update(LightBill lightBill);
        void Delete(LightBill lightBill);


    }
}
