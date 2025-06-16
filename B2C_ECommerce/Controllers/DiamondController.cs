using System;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using B2C_ECommerce.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;

namespace B2C_ECommerce.Controllers
{

    public class DiamondController : Controller
    {
        private readonly IDiamondService _diamondService;

        private readonly ILogger<AccountController> _logger;

        public DiamondController(IDiamondService diamondService, ILogger<AccountController> logger)
        {
            _diamondService = diamondService;
            _logger = logger;
        }

        public IActionResult Index(int? shapeId=0)
        {
            ViewBag.ShapeId = shapeId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDiamondListByFilter(DiamondFilters diamondFilters, int pageNumber = 1, int pageSize = 10)
        {
            var response = await _diamondService.GetDiamondListByFilter(diamondFilters, pageNumber, pageSize);
            return PartialView("_DiamondDataList", response);
        }

        [HttpGet]
        public async Task<IActionResult> GetShapeList()
        {
            try
            {
                var response = await _diamondService.GetShapeListAsync();
                return PartialView("_ShapeList", response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception :", ex.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCaratList()
        {
            var response = await _diamondService.GetCaratListAsync();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCaratSizeDataRangeAsync1()
        {
            var response = await _diamondService.GetCaratSizeDataRangeAsync1();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetColorList()
        {
            var response = await _diamondService.GetColorListAsync();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCutList()
        {
            var response = await _diamondService.GetCutListAsync();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetClarityList()
        {
            var response = await _diamondService.GetClarityListAsync();
            return Json(response);
        }

        [HttpGet]
        public IActionResult GetCertificate(string diamondCerti)
        {
            ViewBag.Certificate = diamondCerti;
            return PartialView("_CertificateDisplay");
        }

        [HttpGet]
        public async Task<IActionResult> GetSingleDiamond(int diamondId, string diamondProparty)
        {
            var diamondDT = await _diamondService.GetDiamondById(diamondId);
            ViewBag.V_OR_I = diamondProparty;
            return View(diamondDT);
        }

        [HttpGet]
        public async Task<IActionResult> GetDiamondsByIds(string diamondIds)
        {
            int[] diamondIdArray = diamondIds.Split(',')
                                  .Select(id => int.Parse(id))
                                  .ToArray();
            var response = await _diamondService.GetSelectedDiamondByIds(diamondIdArray);
            return PartialView("_CompareDiamonds", response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTableRanges()
        {
            var response = await _diamondService.GetTableRangesAsync();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetDepthRanges()
        {
            var response = await _diamondService.GetDepthRangesAsync();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetRatioRanges()
        {
            var response = await _diamondService.GetRatioRangesAsync();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetPolishList()
        {
            var response = await _diamondService.GetPolishListAsync();
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetFluorList()
        {
            var response = await _diamondService.GetFluorListAsync();
            return Json(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetSymmetryList()
        {
            var response = await _diamondService.GetSymmetryListAsync();
            return Json(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetPriceRange()
        {
            var response = await _diamondService.GetProductPriceRangeData();
            return Json(response);
        }
    }
}
