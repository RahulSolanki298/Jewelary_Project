using B2C_ECommerce.IServices;
using Common;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace B2C_ECommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productRepository;
        public ProductsController(IProductService productService)
        {
            _productRepository = productService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string type, string styleId, string collectionId)
        {
            var productStyleColl = new ProductStyleCollectionDTO();

            if (type == "Engagement" && !string.IsNullOrEmpty(styleId))
            {
                ViewBag.Title = "Engagement";
                var styleList = await _productRepository.ProgramStylesList();
                productStyleColl.StyleList = styleList.Where(x=>x.IsActivated==true).ToList();
                productStyleColl.IsStyle = true;
                return View(productStyleColl);
            }
            else if (type == "Wedding" && !string.IsNullOrEmpty(styleId))
            {
                ViewBag.Title = "Wedding";
                var styleList = await _productRepository.ProgramStylesList();
                productStyleColl.StyleList = styleList.Where(x => x.Id == Convert.ToInt32(styleId)).ToList();
                return View(productStyleColl);
            }
            else if (type == "Earrings" && !string.IsNullOrEmpty(styleId))
            {
                ViewBag.Title = "Earrings";
                var styleList = await _productRepository.ProgramStylesList();
                productStyleColl.StyleList = styleList.Where(x => x.Id == Convert.ToInt32(styleId)).ToList();
                return View(productStyleColl);
            }
            else if (!string.IsNullOrEmpty(collectionId))
            {
                ViewBag.Title = "Collection";
                var collectionList = await _productRepository.ProductCollectionList();
                productStyleColl.CollectionList = collectionList.Where(x => x.Id == Convert.ToInt32(collectionId)).ToList();
                return View(productStyleColl);
            }
            else
            {
                var styleList = await _productRepository.ProgramStylesList();
                if (styleId != null)
                {
                    productStyleColl.StyleList = styleList.Where(x => x.Id == Convert.ToInt32(styleId)).ToList();
                }
                else
                {
                    productStyleColl.StyleList = styleList;
                }
                return View(productStyleColl);
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetProductListByFilter(ProductFilters filters, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productRepository.GetProductListByFilter(filters, pageNumber, pageSize);
            return PartialView("_NewImagesAndProducts", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDataFilters()
        {
            var productFilters = new ProductPropertyListDTO();
            var data = await _productRepository.GetProductColorList();
            productFilters.Colors = data.ToList();
            //productFilters.CollectionList = (await _productRepository.GetSubcategoryList());
            var shapes = (await _productRepository.GetShapeList());
            productFilters.Shapes = shapes.ToList();
            //var priceDT = await _productRepository.GetProductPriceRangeData();
            //productFilters.FromPrice = priceDT.MinPrice;
            //productFilters.ToPrice = priceDT.MaxPrice;

            return PartialView("_ProductFilterBar", productFilters);
        }

        [HttpGet]
        public IActionResult GetSelectedProductCompare(string diamondIds)
        {
            return PartialView("_ProductFilterBar", null);
        }


        [HttpGet]
        public async Task<IActionResult> ProductDetails(string productKey)
        {
            var result = await _productRepository.GetProductByProductId(productKey);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetailsByColorId(string sku, int? colorId = 0)
        {
            var jsonResult = await _productRepository.GetProductsByColorId(sku, colorId);
            return Json(jsonResult);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetailsByCaratId(string sku, int? caratId = 0)
        {
            var jsonResult = await _productRepository.GetProductsByColorId(sku, 0, caratId);
            return Json(jsonResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDetailsByShapeId(string sku, int? shapeId = 0, int colorId = 0)
        {
            if (colorId > 0 && shapeId > 0)
            {
                var jsonResult = await _productRepository.GetJewelleryByShapeColorId(sku, colorId, shapeId);
                return Json(jsonResult);
            }

            return null;
        }
    }
}
