using DATA.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DATA.Interfaces
{
    public interface ICustDetails
    {
        IEnumerable<CustDetails> getAll();
    }
}
