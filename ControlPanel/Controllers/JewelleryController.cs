using Business.Repository.IRepository;
using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    public class JewelleryController : Controller
    {
        private readonly IProductRepository _productRepository;

        public JewelleryController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetJewellries()
        {
            var result= await _productRepository.GetProductStyleList();
            return PartialView("_JewelleryList",result);
        }
    }
}
