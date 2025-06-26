using Business.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    public class ProductCollectionsController : Controller
    {
        private readonly ICollectionRepository _collectionRepo;
        public ProductCollectionsController(ICollectionRepository collectionRepository)
        {
            _collectionRepo = collectionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _collectionRepo.GetProductCollections();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCollectionItems()
        {
            var result = await _collectionRepo.GetProductCollectionItemsList();
            return PartialView("_ProductCollectionItemList", result);
        }
    }
}
