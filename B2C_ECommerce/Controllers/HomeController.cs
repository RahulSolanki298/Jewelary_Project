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
        public HomeController(ILogger<HomeController> logger
            , IBlogRepository blogRepository,ISettingRepository settingRepository)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            _settingRepository = settingRepository;
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


        public async Task<IActionResult> HomePageData() { 
        
            var response=await _settingRepository.GetHomePageSettingList();
            var webData=response.Where(x=>x.Device==SD.WebDevice).FirstOrDefault();

            return PartialView("_HomePage",webData);
        }


        //public async IActionResult StylesListData()
        //{

        //    var response = await _settingRepository.GetHomePageSettingList();
        //    var webData = response.Where(x => x.Device == SD.WebDevice).FirstOrDefault();

        //    return PartialView("_HomePage", webData);
        //}

    }
}
