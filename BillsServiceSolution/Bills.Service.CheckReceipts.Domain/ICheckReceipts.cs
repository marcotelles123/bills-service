using Bills.Service.Common.VO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bills.Service.CheckReceipts.Domain
{
    public interface ICheckReceipts
    {
        IList<EmailVO> GetAllReceipts();
    }
}
