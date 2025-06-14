using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using B2C_ECommerce.Models;
using Business.Repository.IRepository;
using DataAccess.Entities;
using Common;

namespace B2C_ECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IProductPropertyRepository _productPropertyRepository;

        public HomeController(ILogger<HomeController> logger, IBlogRepository blogRepository,ISettingRepository settingRepository
            ,IProductPropertyRepository productPropertyRepository)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            _settingRepository = settingRepository;
            _productPropertyRepository = productPropertyRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> GetBlogList()
        {
            var result = await _blogRepository.GetBlogList();
            return PartialView("_BlogList", result);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [HttpGet]
        public async Task<IActionResult> ShowHomePageData() { 
        
            var response=await _settingRepository.GetHomePageSettingList();
            var webData=response.Where(x=>x.Device==SD.WebDevice).FirstOrDefault();

            return PartialView("_HomeHeaders", webData);
        }

        [HttpGet]
        public async Task<IActionResult> ShowApplicationNavmenu()
        {
            var response = await _productPropertyRepository.GetMainPropertyList();
            return PartialView("_HomeHeaders", response);
        }

    }
}
