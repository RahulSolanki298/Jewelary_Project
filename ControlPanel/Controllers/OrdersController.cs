using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IB2COrdersRepository _OrderRepo;

        public OrdersController(IB2COrdersRepository OrderRepo)
        {
            _OrderRepo = OrderRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OrderRequestData()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> OrderRequests()
        {
            var requestList = await _OrderRepo.GetB2COrderRequestList();
            return PartialView("_OrderRequests", requestList);
        }

        [HttpGet]
        public async Task<IActionResult> OrderAccepted()
        {
            var requestList = await _OrderRepo.GetB2COrderAcceptList();
            return PartialView("_OrderRequests", requestList);
        }


        [HttpGet]
        public async Task<IActionResult> OrderRejected()
        {
            var requestList = await _OrderRepo.GetB2COrderRejectList();
            return PartialView("_OrderRequests", requestList);
        }


        [HttpGet]
        public async Task<IActionResult> OrderStartPackList()
        {
            var requestList = await _OrderRepo.GetB2COrderPackageList();
            return PartialView("_OrderRequests", requestList);
        }

        [HttpGet]
        public async Task<IActionResult> OrderReadyForShipmentList()
        {
            var requestList = await _OrderRepo.GetB2COrderReadyForShipmentList();
            return PartialView("_OrderRequests", requestList);
        }

        [HttpGet]
        public async Task<IActionResult> OrderShipped()
        {
            var requestList = await _OrderRepo.GetB2COrderShippedList();
            return PartialView("_OrderRequests", requestList);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDelivered()
        {
            var requestList = await _OrderRepo.GetB2COrderDeliveredList();
            return PartialView("_OrderRequests", requestList);
        }

        [HttpGet]
        public async Task<IActionResult> OrderCompleted()
        {
            var requestList = await _OrderRepo.GetB2COrderComplatedList();
            return PartialView("_OrderRequests", requestList);
        }
    }
}
