using B2C_ECommerce.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace B2C_ECommerce.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IDiamondService _diamondService;

        public WishlistController(IDiamondService diamondService)
        {
                _diamondService = diamondService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetWishListByIds(string diamondIds)
        {
            int[] diamondIdArray = diamondIds.Split(',')
                                  .Select(id => int.Parse(id))
                                  .ToArray();
            var response = await _diamondService.GetSelectedDiamondByIds(diamondIdArray);
            return PartialView("_WishlistDiamonds", response);
        }
    }
}
