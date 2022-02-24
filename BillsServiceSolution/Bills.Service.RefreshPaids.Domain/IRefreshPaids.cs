using System;
using System.Collections.Generic;
using System.Text;

namespace Bills.Service.RefreshPaids.Domain
{
    public interface IRefreshPaids
    {
        bool Execute();
    }
}
