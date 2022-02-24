using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bills.Service.RefreshPaids.Infra
{
    public interface IRefreshPaidsRepository
    {
        IEnumerable<Common.VO.BillsVO> GetAllResults();
        object SetAsPaidAsync(ObjectId id);
        object SetAsUnPaidAsync(ObjectId id);
    }
}
