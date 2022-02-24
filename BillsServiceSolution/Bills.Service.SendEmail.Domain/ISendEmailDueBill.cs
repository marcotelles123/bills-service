using System;
using System.Collections.Generic;
using System.Text;

namespace Bills.Service.SendEmail.Domain
{
    public interface ISendEmailDueBill
    {
        bool Execute();
    }
}
