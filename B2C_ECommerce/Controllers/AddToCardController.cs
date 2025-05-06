using Microsoft.AspNetCore.Mvc;

namespace B2C_ECommerce.Controllers
{

    [Route("JewelFacet/AddToCard")]
    public class AddToCardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
