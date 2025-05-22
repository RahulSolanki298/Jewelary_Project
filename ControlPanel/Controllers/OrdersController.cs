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
        public async Task<IActionResult> OrderRequests()
        {
            var requestList = await _OrderRepo.GetB2COrderRequestList();
            return View(requestList);
        }

        [HttpGet]
        public IActionResult OrderProcessList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OrderRejectList()
        {
            return View();
        }


        [HttpGet]
        public IActionResult OrderCancelList()
        {
            return View();
        }

    }
}
