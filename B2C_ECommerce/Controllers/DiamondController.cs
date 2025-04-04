using System.Threading.Tasks;
using B2C_ECommerce.IServices;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace B2C_ECommerce.Controllers
{
    public class DiamondController : Controller
    {
        private readonly IDiamondService _diamondService;
        public DiamondController(IDiamondService diamondService)
        {
            _diamondService = diamondService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDiamondList(DiamondFilters diamondFilters, int pageNumber = 1, int pageSize = 10)
        {
            var response = await _diamondService.GetDiamondListByFilter(diamondFilters, pageNumber, pageSize);
            return PartialView("~/Views/Diamond/_DiamondDataList.cshtml", response);
        }

        [HttpGet]
        public async Task<IActionResult> GetSingleDiamond(int diamondId, string diamondProparty)
        {
            var diamondDT= await _diamondService.GetDiamondById(diamondId);
            ViewBag.V_OR_I = diamondProparty;
            return View(diamondDT);
        }
    }
}
