using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Repository.IRepository
{
    public interface IB2COrdersRepository
    {
        Task<IEnumerable<CustomerOrders>> GetB2COrderAcceptList();

        Task<IEnumerable<CustomerOrders>> GetB2COrderCancleList();

        Task<IEnumerable<CustomerOrders>> GetB2COrderProcessingList();

        Task<IEnumerable<CustomerOrders>> GetB2COrderRejectList();

        Task<IEnumerable<CustomerOrders>> GetB2COrderDeliveredList();

        Task<IEnumerable<CustomerOrders>> GetB2COrderComplatedList();

        Task<IEnumerable<CustomerOrders>> GetB2COrderRequestList();

        Task<IEnumerable<CustomerOrders>> GetB2COrderReadyForShipmentList();

        Task<IEnumerable<CustomerOrders>> GetB2COrderShippedList();
    }
}
