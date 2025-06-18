using B2C_ECommerce.IServices;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;
using System.Threading.Tasks;

namespace B2C_ECommerce.Controllers
{

    [Route("JewelFacet/AddToCard")]
    public class AddToCardController : Controller
    {
        private readonly IDiamondService _diamondService;
        private readonly IProductService _productService;

        public AddToCardController(IDiamondService diamondService, IProductService productService)
        {
            _diamondService = diamondService;
            _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetAdd2CardListByIds(string diamondIds, string productIds)
        {
            AddToCardModal data = new AddToCardModal();

            if (!string.IsNullOrEmpty(diamondIds) && diamondIds.Length > 0)
            {
                int[] diamondIdArray = diamondIds.Split(',')
                                  .Select(id => int.Parse(id))
                                  .ToArray();
                data.Diamonds = await _diamondService.GetSelectedDiamondByIds(diamondIdArray);
            }

            if (!string.IsNullOrEmpty(productIds) && productIds.Length > 0)
            {
                string[] productIdArray = productIds.Split(',')
                                  .Select(id => id)
                                  .ToArray();
                data.Jewelleries = await _productService.GetSelectedProductByIds(productIdArray);
            }

            return PartialView("~/Views/Shared/_AddToCard.cshtml", data);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAdd2CardListByIds(string diamondIds, string productIds)
        {
            AddToCardModal data = new AddToCardModal();

            if (!string.IsNullOrEmpty(diamondIds) && diamondIds.Length > 0)
            {
                int[] diamondIdArray = diamondIds.Split(',')
                                  .Select(id => int.Parse(id))
                                  .ToArray();
                data.Diamonds = await _diamondService.GetSelectedDiamondByIds(diamondIdArray);
            }

            if (!string.IsNullOrEmpty(productIds) && productIds.Length > 0)
            {
                string[] productIdArray = productIds.Split(',')
                                  .Select(id => id)
                                  .ToArray();
                data.Jewelleries = await _productService.GetSelectedProductByIds(productIdArray);
            }

            return PartialView("_Add2CardProduct", data);
        }


    }
}
