using DATA.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DATA.Interfaces
{
    public interface ILineMaster
    {
        LineMaster GetLinebyId(LineMaster lineMaster);
        List<LineMaster> GetAllLines();

        List<LineMaster> GetAllLineS();
    }
}
