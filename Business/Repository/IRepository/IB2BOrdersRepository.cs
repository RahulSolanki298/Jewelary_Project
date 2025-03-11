using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IB2BOrdersRepository
    {
        Task<IEnumerable<Orders>> GetB2BOrderReqs();
    }
}
