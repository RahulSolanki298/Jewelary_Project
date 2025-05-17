using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    public class DiamondController : Controller
    {
        private readonly IDiamondRepository _diamondRepository;

        public DiamondController(IDiamondRepository diamondRepository)
        {
                _diamondRepository= diamondRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DiamondList()
        {
            var data= await _diamondRepository.GetDiamondList();
            return PartialView("_DiamondList", data);
        }


        [HttpGet]
        public IActionResult DiamondProperty()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpsertProperty(DiamondData diamondData)
        {
            return View();
        }
        
    }
}
