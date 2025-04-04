using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository
{
    public class B2COrdersRepository : IB2COrdersRepository
    {
        private readonly ApplicationDBContext _context;

        public B2COrdersRepository(ApplicationDBContext context) 
        {
            _context = context;        
        }

        public async Task<IEnumerable<CustomerOrders>> GetB2COrderAcceptList()
        {
            return await (from ord in _context.CustomerOrders
                          join sts in _context.CustomerOrderStatus on ord.OrderId.ToString() equals sts.OrderId
                          join status in _context.OrderStatus on sts.CurrentStatusId equals status.Id
                          where status.Name == SD.Requested
                          select ord).ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrders>> GetB2COrderCancleList()
        {
            return await (from ord in _context.CustomerOrders
                         join sts in _context.CustomerOrderStatus on ord.OrderId.ToString() equals sts.OrderId
                         join status in _context.OrderStatus on sts.CurrentStatusId equals status.Id
                         where status.Name == SD.Cancelled
                         select ord).ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrders>> GetB2COrderProcessingList()
        {
            return await (from ord in _context.CustomerOrders
                          join sts in _context.CustomerOrderStatus on ord.OrderId.ToString() equals sts.OrderId
                          join status in _context.OrderStatus on sts.CurrentStatusId equals status.Id
                          where status.Name == SD.Processing
                          select ord).ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrders>> GetB2COrderRejectList()
        {
            return await (from ord in _context.CustomerOrders
                          join sts in _context.CustomerOrderStatus on ord.OrderId.ToString() equals sts.OrderId
                          join status in _context.OrderStatus on sts.CurrentStatusId equals status.Id
                          where status.Name == SD.OrderRejected
                          select ord).ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrders>> GetB2COrderReadyForShipmentList()
        {
            return await (from ord in _context.CustomerOrders
                          join sts in _context.CustomerOrderStatus on ord.OrderId.ToString() equals sts.OrderId
                          join status in _context.OrderStatus on sts.CurrentStatusId equals status.Id
                          where status.Name == SD.ReadyForShipment
                          select ord).ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrders>> GetB2COrderShippedList()
        {
            return await (from ord in _context.CustomerOrders
                          join sts in _context.CustomerOrderStatus on ord.OrderId.ToString() equals sts.OrderId
                          join status in _context.OrderStatus on sts.CurrentStatusId equals status.Id
                          where status.Name == SD.Shipped
                          select ord).ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrders>> GetB2COrderDeliveredList()
        {
            return await (from ord in _context.CustomerOrders
                          join sts in _context.CustomerOrderStatus on ord.OrderId.ToString() equals sts.OrderId
                          join status in _context.OrderStatus on sts.CurrentStatusId equals status.Id
                          where status.Name == SD.Delivered
                          select ord).ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrders>> GetB2COrderComplatedList()
        {
            return await (from ord in _context.CustomerOrders
                          join sts in _context.CustomerOrderStatus on ord.OrderId.ToString() equals sts.OrderId
                          join status in _context.OrderStatus on sts.CurrentStatusId equals status.Id
                          where status.Name == SD.Complated
                          select ord).ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrders>> GetB2COrderRequestList()
        {
            return await(from ord in _context.CustomerOrders
                         join sts in _context.CustomerOrderStatus on ord.OrderId.ToString() equals sts.OrderId
                         join status in _context.OrderStatus on sts.CurrentStatusId equals status.Id
                         where status.Name == SD.Requested
                         select ord).ToListAsync();
        }
    }
}
