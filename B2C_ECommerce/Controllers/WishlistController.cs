using B2C_ECommerce.IServices;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace B2C_ECommerce.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IDiamondService _diamondService;
        private readonly IProductService _productService;

        public WishlistController(IDiamondService diamondService,IProductService productService)
        {
                _diamondService = diamondService;
                _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetWishListByIds(string diamondIds, string productIds)
        {
            var wishlist = new WishlistModel();

            // Parse diamond IDs
            var diamondIdArray = ParseIds(diamondIds);
            if (diamondIdArray.Length > 0)
            {
                wishlist.Diamonds = await _diamondService.GetSelectedDiamondByIds(diamondIdArray);
            }

            // Parse product IDs
            var productIdArray = ParseIds(productIds);
            if (productIdArray.Length > 0)
            {
                wishlist.Jewelleries = await _productService.GetSelectedProductByIds(productIdArray);
            }

            return PartialView("_WishlistDiamonds", wishlist);
        }

        private int[] ParseIds(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids)) return Array.Empty<int>();

            return ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(id => id.Trim())
                      .Where(id => int.TryParse(id, out _))
                      .Select(int.Parse)
                      .ToArray();
        }

    }
}
