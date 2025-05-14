using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace B2C_ECommerce.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class B2COrdersController : ControllerBase
    {
        private readonly IB2COrdersRepository _orders;

        public B2COrdersController(IB2COrdersRepository orders)
        {
            _orders = orders;  
        }

        [HttpGet("customer-order-request")]
        public IActionResult GetOrderList()
        {
            var ordData= _orders.GetB2COrderRequestList();
            return Ok(ordData);
        }
    }
}
