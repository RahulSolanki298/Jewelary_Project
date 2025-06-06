using Business.Repository.IRepository;
using Common;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ControlPanel.Controllers
{

    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize]
    public class JewelleryController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductPropertyRepository _productPropertyRepository;
        private readonly IWebHostEnvironment _env;
        public JewelleryController(IProductRepository productRepository, IProductPropertyRepository productProperty, IWebHostEnvironment env)
        {
            _productRepository = productRepository;
            _productPropertyRepository = productProperty;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetJewellries()
        {
            var productList = await _productRepository.GetProductStyleList();
            return PartialView("_JewelleryList", productList);
        }

        [HttpGet]
        public IActionResult UploadJewellery()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExcelUpload(IFormFile file)
        {
            var productUpload = new List<ProductDTO>();

            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "No file uploaded.";
                return PartialView("_JewelleryList", productUpload);
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "Invalid file format. Please upload an Excel (.xlsx) file.";
                return PartialView("_ExcelUploadList", productUpload);
            }

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(stream);

                // Map worksheet names to processors
                var sheetProcessors = new Dictionary<string, Func<ExcelWorksheet, List<ProductDTO>>>(StringComparer.OrdinalIgnoreCase)
                {
                    { "engagement rings", ProcessRingData },
                    { "wedding bands", ProcessBandsData },
                    { "earrings", ProcessEarringsData },
                    { "pendants", ProcessPendantsData },
                    { "bracelets", ProcessBraceletsData }
                };

                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    string sheetName = worksheet.Name.Trim().ToLower();

                    if (sheetProcessors.TryGetValue(sheetName, out var processor))
                    {
                        var processedItems = processor(worksheet);
                        productUpload.AddRange(processedItems);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred while processing the Excel file: {ex.Message}";
            }

            return PartialView("_ExcelUploadList", productUpload);
        }


        [HttpPost]
        public async Task<IActionResult> SaveAllProduct(string productsJson)
        {
            if (string.IsNullOrEmpty(productsJson))
            {
                return Json("Product Doesn't Found");
            }

            var products = JsonConvert.DeserializeObject<List<ProductDTO>>(productsJson);

            var categoryList = products.GroupBy(p => p.CategoryName)
                                            .Select(g => g.First())
                                            .ToList();

            foreach (var wkSheet in categoryList)
            {

                if (wkSheet.CategoryName.Trim().ToLower() == "rings")
                {
                    var ringProducts = products.Where(x => x.CategoryName == "Rings").ToList();
                    await _productRepository.SaveNewProductList(ringProducts, "Rings");
                }
                else if (wkSheet.CategoryName.Trim().ToLower() == "bands")
                {
                    var weddings = products.Where(x => x.CategoryName == "Bands").ToList();
                    await _productRepository.SaveNewProductList(weddings, "Bands");
                }
                else if (wkSheet.CategoryName.Trim().ToLower() == "earrings")
                {
                    var weddings = products.Where(x => x.CategoryName == "Earrings").ToList();
                    await _productRepository.SaveEarringsList(weddings, "Earrings");
                }
                else if (wkSheet.CategoryName.Trim().ToLower() == "pendants")
                {
                    var weddings = products.Where(x => x.CategoryName == "Pendants").ToList();
                    await _productRepository.SaveEarringsList(weddings, "Pendants"); // Hypothetical method
                }
                else if (wkSheet.CategoryName.Trim().ToLower() == "bracelets")
                {
                    var weddings = products.Where(x => x.CategoryName == "Bracelets").ToList();
                    await _productRepository.SaveEarringsList(weddings, "Bracelets"); // Hypothetical method
                }
            }

            return Json("Data Uploaded Successfully.");
        }


        #region Rings Data

        private List<ProductDTO> ProcessRingData(ExcelWorksheet worksheet)
        {
            var newProductList = new List<ProductDTO>();
            var products = new List<ProductDTO>();
            var tempProducts = new ProductDTO();
            int rowCount = worksheet.Dimension.Rows;
            ProductDTO product = null;

            for (int row = 5; row <= rowCount; row++)
            {
                product = new ProductDTO
                {
                    CategoryName = SD.Rings,
                    Title = worksheet.Cells[row, 5].Text,
                    VenderName = worksheet.Cells[row, 6].Text,
                    VenderStyle = worksheet.Cells[row, 7].Text,
                    Sku = worksheet.Cells[row, 7].Text,
                    Diameter = worksheet.Cells[row, 8].Text,
                    Length = worksheet.Cells[row, 9].Text,
                    BandWidth = worksheet.Cells[row, 10].Text,
                    GoldWeight = worksheet.Cells[row, 11].Text,
                    CTW = worksheet.Cells[row, 12].Text,
                    CenterShapeName = worksheet.Cells[row, 13].Text,
                    CenterCaratName = worksheet.Cells[row, 14].Text,
                    Certificate = worksheet.Cells[row, 15].Text,
                    ColorName = worksheet.Cells[row, 16].Text,
                    StyleName = worksheet.Cells[row, 17].Text,
                    AccentStoneShapeName = worksheet.Cells[row, 18].Text,
                    MMSize = worksheet.Cells[row, 19].Text,
                    DiaWT = decimal.TryParse(worksheet.Cells[row, 20].Text, out var diaWt) ? diaWt : (decimal?)null,
                    NoOfStones = int.TryParse(worksheet.Cells[row, 21].Text, out var noOfStones) ? noOfStones : 0,
                    Grades = worksheet.Cells[row, 22].Text,
                    ProductType = worksheet.Cells[row, 23].Text,
                    Price = ConvertStringToDecimal(worksheet.Cells[row, 24].Text),
                    UnitPrice = ConvertStringToDecimal(worksheet.Cells[row, 24].Text),
                    Description = worksheet.Cells[row, 33].Text,
                    WholesaleCost = decimal.TryParse(worksheet.Cells[row, 27].Text, out var hCost) ? hCost : (decimal?)null,
                    IsReadyforShip = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text) && worksheet.Cells[row, 1].Text == SD.ReadyToPdp ? true : false,
                };

                if (!string.IsNullOrWhiteSpace(product.EventName) && !string.IsNullOrWhiteSpace(product.VenderName) && !string.IsNullOrWhiteSpace(product.VenderStyle) && !string.IsNullOrWhiteSpace(product.Sku))
                {
                    tempProducts = new ProductDTO();
                    tempProducts = product;
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);
                }
                else
                {
                    product.CategoryName = SD.Rings;
                    product.Title = string.IsNullOrWhiteSpace(product.Title) ? tempProducts.Title : product.Title;
                    product.VenderName = string.IsNullOrWhiteSpace(product.VenderName) ? tempProducts.VenderName : product.VenderName;
                    product.VenderStyle = string.IsNullOrWhiteSpace(product.VenderStyle) ? tempProducts.VenderStyle : product.VenderStyle;
                    product.Sku = string.IsNullOrWhiteSpace(product.Sku) ? tempProducts.Sku : product.Sku;
                    product.Length = string.IsNullOrWhiteSpace(product.Length) ? tempProducts.Length : product.Length;
                    product.BandWidth = string.IsNullOrWhiteSpace(product.BandWidth) ? tempProducts.BandWidth : product.BandWidth;
                    product.GoldWeight = string.IsNullOrWhiteSpace(product.GoldWeight) ? tempProducts.GoldWeight : product.GoldWeight;
                    product.CTW = string.IsNullOrWhiteSpace(product.CTW) ? tempProducts.CTW : product.CTW;
                    product.CenterShapeName = string.IsNullOrWhiteSpace(product.CenterShapeName) ? tempProducts.CenterShapeName : product.CenterShapeName;
                    product.CenterCaratName = string.IsNullOrWhiteSpace(product.CenterCaratName) ? tempProducts.CenterCaratName : product.CenterCaratName;
                    product.Certificate = string.IsNullOrWhiteSpace(product.Certificate) ? tempProducts.Certificate : product.Certificate;
                    product.StyleName = string.IsNullOrWhiteSpace(product.StyleName) ? tempProducts.StyleName : product.StyleName;
                    product.ColorName = string.IsNullOrWhiteSpace(product.ColorName) ? tempProducts.ColorName : product.ColorName;
                    product.AccentStoneShapeName = string.IsNullOrWhiteSpace(product.AccentStoneShapeName) ? tempProducts.AccentStoneShapeName : product.AccentStoneShapeName;
                    product.MMSize = string.IsNullOrWhiteSpace(product.MMSize) ? tempProducts.MMSize : product.MMSize;
                    product.DiaWT = product.DiaWT == 0 ? tempProducts.DiaWT : product.DiaWT;
                    product.NoOfStones = product.NoOfStones == 0 ? tempProducts.NoOfStones : product.NoOfStones;
                    product.Grades = string.IsNullOrWhiteSpace(product.Grades) ? tempProducts.Grades : product.Grades;
                    product.ProductType = string.IsNullOrWhiteSpace(product.ProductType) ? tempProducts.ProductType : product.ProductType;
                    product.Price = product.Price == 0 ? tempProducts.Price : product.Price;
                    product.UnitPrice = product.Price == 0 ? tempProducts.UnitPrice : product.UnitPrice;
                    product.Description = string.IsNullOrWhiteSpace(product.Description) ? tempProducts.Description : product.Description;
                    product.WholesaleCost = product.WholesaleCost > 0 ? tempProducts.WholesaleCost : product.WholesaleCost;
                    product.IsReadyforShip = product.IsReadyforShip != true ? tempProducts.IsReadyforShip : product.IsReadyforShip;
                    products.Add(product);
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);
                }
            }

            return products;
        }

        private List<ProductDTO> ProcessBandsData(ExcelWorksheet worksheet)
        {
            var newProductList = new List<ProductDTO>();
            var products = new List<ProductDTO>();
            var tempProducts = new ProductDTO();
            int rowCount = worksheet.Dimension.Rows;
            ProductDTO product = null;

            for (int row = 5; row <= rowCount; row++)
            {
                product = new ProductDTO
                {
                    CategoryName = SD.Bands,
                    Title = worksheet.Cells[row, 5].Text,
                    EventName = worksheet.Cells[row, 5].Text,
                    VenderName = worksheet.Cells[row, 6].Text,
                    VenderStyle = worksheet.Cells[row, 7].Text,
                    Sku = worksheet.Cells[row, 7].Text,
                    Diameter = worksheet.Cells[row, 8].Text,
                    Length = worksheet.Cells[row, 9].Text,
                    BandWidth = worksheet.Cells[row, 10].Text,
                    GoldWeight = worksheet.Cells[row, 11].Text,
                    CTW = worksheet.Cells[row, 12].Text,
                    CenterShapeName = worksheet.Cells[row, 13].Text,
                    CenterCaratName = worksheet.Cells[row, 14].Text,
                    ColorName = worksheet.Cells[row, 15].Text,
                    AccentStoneShapeName = worksheet.Cells[row, 17].Text,
                    MMSize = worksheet.Cells[row, 18].Text,
                    DiaWT = decimal.TryParse(worksheet.Cells[row, 19].Text, out var diaWt) ? diaWt : 0,
                    NoOfStones = int.TryParse(worksheet.Cells[row, 20].Text, out var noOfStones) ? noOfStones : 0,
                    Grades = worksheet.Cells[row, 21].Text,
                    ProductType = worksheet.Cells[row, 22].Text,
                    Price = ConvertStringToDecimal(worksheet.Cells[row, 23].Text),
                    UnitPrice = ConvertStringToDecimal(worksheet.Cells[row, 23].Text),
                    Description = worksheet.Cells[row, 32].Text,
                    WholesaleCost = decimal.TryParse(worksheet.Cells[row, 26].Text, out var hCost) ? hCost : 0,
                    IsReadyforShip = string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text) && worksheet.Cells[row, 1].Text == SD.ReadyToPdp ? true : false,
                };

                if (!string.IsNullOrWhiteSpace(product.EventName) && !string.IsNullOrWhiteSpace(product.VenderName) && !string.IsNullOrWhiteSpace(product.VenderStyle) && !string.IsNullOrWhiteSpace(product.Sku))
                {
                    tempProducts = new ProductDTO();
                    tempProducts = product;
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);
                }
                else
                {
                    product.CategoryName = SD.Bands;
                    product.EventName = string.IsNullOrWhiteSpace(product.EventName) ? tempProducts.EventName : product.EventName;
                    product.Title = string.IsNullOrWhiteSpace(product.EventName) ? tempProducts.EventName : product.EventName;
                    product.VenderName = string.IsNullOrWhiteSpace(product.VenderName) ? tempProducts.VenderName : product.VenderName;
                    product.VenderStyle = string.IsNullOrWhiteSpace(product.VenderStyle) ? tempProducts.VenderStyle : product.VenderStyle;
                    product.Sku = string.IsNullOrWhiteSpace(product.Sku) ? tempProducts.Sku : product.Sku;
                    product.Diameter = string.IsNullOrWhiteSpace(product.Diameter) ? tempProducts.Diameter : product.Diameter;
                    product.GoldWeight = string.IsNullOrWhiteSpace(product.GoldWeight) ? tempProducts.GoldWeight : product.GoldWeight;
                    product.CTW = string.IsNullOrWhiteSpace(product.CTW) ? tempProducts.CTW : product.CTW;
                    product.ColorName = string.IsNullOrWhiteSpace(product.ColorName) ? tempProducts.ColorName : product.ColorName;
                    product.AccentStoneShapeName = string.IsNullOrWhiteSpace(product.AccentStoneShapeName) ? tempProducts.AccentStoneShapeName : product.AccentStoneShapeName;
                    product.MMSize = string.IsNullOrWhiteSpace(product.MMSize) ? tempProducts.MMSize : product.MMSize;
                    product.DiaWT = product.DiaWT == 0 ? tempProducts.DiaWT : product.DiaWT;
                    product.NoOfStones = product.NoOfStones == 0 ? tempProducts.NoOfStones : product.NoOfStones;
                    product.Grades = string.IsNullOrWhiteSpace(product.Grades) ? tempProducts.Sku : product.Grades;
                    product.ProductType = string.IsNullOrWhiteSpace(product.ProductType) ? tempProducts.ProductType : product.ProductType;
                    product.Price = product.Price == 0 ? tempProducts.Price : product.Price;
                    product.UnitPrice = product.Price == 0 ? tempProducts.UnitPrice : product.UnitPrice;
                    product.Description = string.IsNullOrWhiteSpace(product.Description) ? tempProducts.Description : product.Description;
                    product.WholesaleCost = product.WholesaleCost > 0 ? tempProducts.WholesaleCost : product.WholesaleCost;
                    product.IsReadyforShip = product.IsReadyforShip != true ? tempProducts.IsReadyforShip : product.IsReadyforShip;
                    products.Add(product);
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);

                }
            }

            return products;
        }

        private List<ProductDTO> ProcessEarringsData(ExcelWorksheet worksheet)
        {
            var newProductList = new List<ProductDTO>();
            var products = new List<ProductDTO>();
            var tempProducts = new ProductDTO();
            int rowCount = worksheet.Dimension.Rows;
            ProductDTO product = null;

            for (int row = 5; row <= rowCount; row++)
            {
                product = new ProductDTO
                {
                    CategoryName = SD.Earrings,
                    Title = worksheet.Cells[row, 5].Text,
                    EventName = worksheet.Cells[row, 5].Text,
                    VenderName = worksheet.Cells[row, 6].Text,
                    VenderStyle = worksheet.Cells[row, 7].Text,
                    Sku = worksheet.Cells[row, 7].Text,
                    Diameter = worksheet.Cells[row, 8].Text,
                    GoldWeight = worksheet.Cells[row, 11].Text,
                    CTW = worksheet.Cells[row, 12].Text,
                    ColorName = worksheet.Cells[row, 15].Text,
                    AccentStoneShapeName = worksheet.Cells[row, 17].Text,
                    MMSize = worksheet.Cells[row, 18].Text,
                    DiaWT = decimal.TryParse(worksheet.Cells[row, 19].Text, out var diaWt) ? diaWt : 0,
                    NoOfStones = int.TryParse(worksheet.Cells[row, 20].Text, out var noOfStones) ? noOfStones : 0,
                    Grades = worksheet.Cells[row, 21].Text,
                    ProductType = worksheet.Cells[row, 22].Text,
                    Price = ConvertStringToDecimal(worksheet.Cells[row, 23].Text),
                    UnitPrice = ConvertStringToDecimal(worksheet.Cells[row, 23].Text),
                    Description = worksheet.Cells[row, 32].Text,
                    WholesaleCost = decimal.TryParse(worksheet.Cells[row, 26].Text, out var hCost) ? hCost : 0,
                    IsReadyforShip = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text) && worksheet.Cells[row, 1].Text == SD.ReadyToPdp ? true : false,
                };

                if (!string.IsNullOrWhiteSpace(product.EventName) && !string.IsNullOrWhiteSpace(product.VenderName) && !string.IsNullOrWhiteSpace(product.VenderStyle) && !string.IsNullOrWhiteSpace(product.Sku))
                {
                    tempProducts = new ProductDTO();
                    tempProducts = product;
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);
                }
                else
                {
                    product.CategoryName = SD.Earrings;
                    product.Diameter = string.IsNullOrWhiteSpace(product.Diameter) ? tempProducts.Diameter : product.Diameter;
                    product.EventName = string.IsNullOrWhiteSpace(product.EventName) ? tempProducts.EventName : product.EventName;
                    product.Title = string.IsNullOrWhiteSpace(product.Title) ? tempProducts.Title : product.Title;
                    product.VenderName = string.IsNullOrWhiteSpace(product.VenderName) ? tempProducts.VenderName : product.VenderName;
                    product.VenderStyle = string.IsNullOrWhiteSpace(product.VenderStyle) ? tempProducts.VenderStyle : product.VenderStyle;
                    product.Sku = string.IsNullOrWhiteSpace(product.Sku) ? tempProducts.Sku : product.Sku;
                    product.BandWidth = string.IsNullOrWhiteSpace(product.BandWidth) ? tempProducts.BandWidth : product.BandWidth;
                    product.GoldWeight = string.IsNullOrWhiteSpace(product.GoldWeight) ? tempProducts.GoldWeight : product.GoldWeight;
                    product.CTW = string.IsNullOrWhiteSpace(product.CTW) ? tempProducts.CTW : product.CTW;
                    product.ColorName = string.IsNullOrWhiteSpace(product.ColorName) ? tempProducts.ColorName : product.ColorName;
                    product.AccentStoneShapeName = string.IsNullOrWhiteSpace(product.AccentStoneShapeName) ? tempProducts.AccentStoneShapeName : product.AccentStoneShapeName;
                    product.MMSize = string.IsNullOrWhiteSpace(product.MMSize) ? tempProducts.MMSize : product.MMSize;
                    product.DiaWT = product.DiaWT == 0 ? tempProducts.DiaWT : product.DiaWT;
                    product.NoOfStones = product.NoOfStones == 0 ? tempProducts.NoOfStones : product.NoOfStones;
                    product.Grades = string.IsNullOrWhiteSpace(product.Grades) ? tempProducts.Grades : product.Grades;
                    product.ProductType = string.IsNullOrWhiteSpace(product.ProductType) ? tempProducts.ProductType : product.ProductType;
                    product.Price = product.Price == 0 ? tempProducts.Price : product.Price;
                    product.UnitPrice = product.Price == 0 ? tempProducts.UnitPrice : product.UnitPrice;
                    product.Description = string.IsNullOrWhiteSpace(product.Description) ? tempProducts.Description : product.Description;
                    product.WholesaleCost = product.WholesaleCost > 0 ? tempProducts.WholesaleCost : product.WholesaleCost;
                    product.IsReadyforShip = product.IsReadyforShip != true ? tempProducts.IsReadyforShip : product.IsReadyforShip;
                    products.Add(product);
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);

                }
            }

            return products;
        }

        private List<ProductDTO> ProcessPendantsData(ExcelWorksheet worksheet)
        {
            var newProductList = new List<ProductDTO>();
            var products = new List<ProductDTO>();
            var tempProducts = new ProductDTO();
            int rowCount = worksheet.Dimension.Rows;
            ProductDTO product = null;

            for (int row = 5; row <= rowCount; row++)
            {
                product = new ProductDTO
                {
                    CategoryName = SD.Pendants,
                    Title = worksheet.Cells[row, 5].Text,
                    EventName = worksheet.Cells[row, 5].Text,
                    VenderName = worksheet.Cells[row, 6].Text,
                    VenderStyle = worksheet.Cells[row, 7].Text,
                    Sku = worksheet.Cells[row, 7].Text,
                    Length = worksheet.Cells[row, 9].Text,
                    GoldWeight = worksheet.Cells[row, 11].Text,
                    CTW = worksheet.Cells[row, 12].Text,
                    ColorName = worksheet.Cells[row, 15].Text,
                    AccentStoneShapeName = worksheet.Cells[row, 17].Text,
                    MMSize = worksheet.Cells[row, 18].Text,
                    DiaWT = decimal.TryParse(worksheet.Cells[row, 19].Text, out var diaWt) ? diaWt : 0,
                    NoOfStones = int.TryParse(worksheet.Cells[row, 20].Text, out var noOfStones) ? noOfStones : 0,
                    Grades = worksheet.Cells[row, 21].Text,
                    ProductType = worksheet.Cells[row, 22].Text,
                    Price = ConvertStringToDecimal(worksheet.Cells[row, 23].Text),
                    UnitPrice = ConvertStringToDecimal(worksheet.Cells[row, 23].Text),
                    Description = worksheet.Cells[row, 32].Text,
                    WholesaleCost = decimal.TryParse(worksheet.Cells[row, 26].Text, out var hCost) ? hCost : 0,
                    IsReadyforShip = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text) && worksheet.Cells[row, 1].Text == SD.ReadyToPdp ? true : false
                };

                if (!string.IsNullOrWhiteSpace(product.EventName) && !string.IsNullOrWhiteSpace(product.VenderName) && !string.IsNullOrWhiteSpace(product.VenderStyle) && !string.IsNullOrWhiteSpace(product.Sku))
                {
                    tempProducts = new ProductDTO();
                    tempProducts = product;
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);
                }
                else
                {
                    product.CategoryName = SD.Pendants;
                    product.EventName = string.IsNullOrWhiteSpace(product.EventName) ? tempProducts.EventName : product.EventName;
                    product.Title = string.IsNullOrWhiteSpace(product.Title) ? tempProducts.Title : product.Title;
                    product.VenderName = string.IsNullOrWhiteSpace(product.VenderName) ? tempProducts.VenderName : product.VenderName;
                    product.VenderStyle = string.IsNullOrWhiteSpace(product.VenderStyle) ? tempProducts.VenderStyle : product.VenderStyle;
                    product.Sku = string.IsNullOrWhiteSpace(product.Sku) ? tempProducts.Sku : product.Sku;
                    product.Length = string.IsNullOrWhiteSpace(product.Length) ? tempProducts.Length : product.Length;
                    product.GoldWeight = string.IsNullOrWhiteSpace(product.GoldWeight) ? tempProducts.GoldWeight : product.GoldWeight;
                    product.CTW = string.IsNullOrWhiteSpace(product.CTW) ? tempProducts.CTW : product.CTW;
                    product.ColorName = string.IsNullOrWhiteSpace(product.ColorName) ? tempProducts.ColorName : product.ColorName;
                    product.AccentStoneShapeName = string.IsNullOrWhiteSpace(product.AccentStoneShapeName) ? tempProducts.AccentStoneShapeName : product.AccentStoneShapeName;
                    product.MMSize = string.IsNullOrWhiteSpace(product.MMSize) ? tempProducts.MMSize : product.MMSize;
                    product.DiaWT = product.DiaWT == 0 ? tempProducts.DiaWT : product.DiaWT;
                    product.NoOfStones = product.NoOfStones == 0 ? tempProducts.NoOfStones : product.NoOfStones;
                    product.Grades = string.IsNullOrWhiteSpace(product.Grades) ? tempProducts.Grades : product.Grades;
                    product.ProductType = string.IsNullOrWhiteSpace(product.ProductType) ? tempProducts.ProductType : product.ProductType;
                    product.Price = product.Price == 0 ? tempProducts.Price : product.Price;
                    product.UnitPrice = product.Price == 0 ? tempProducts.UnitPrice : product.UnitPrice;
                    product.Description = string.IsNullOrWhiteSpace(product.Description) ? tempProducts.Description : product.Description;
                    product.WholesaleCost = product.WholesaleCost > 0 ? tempProducts.WholesaleCost : product.WholesaleCost;
                    product.IsReadyforShip = product.IsReadyforShip != true ? tempProducts.IsReadyforShip : product.IsReadyforShip;
                    products.Add(product);
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);

                }
            }

            return products;
        }

        private List<ProductDTO> ProcessBraceletsData(ExcelWorksheet worksheet)
        {
            var newProductList = new List<ProductDTO>();
            var products = new List<ProductDTO>();
            var tempProducts = new ProductDTO();
            int rowCount = worksheet.Dimension.Rows;
            ProductDTO product = null;
            for (int row = 5; row <= rowCount; row++)
            {
                product = new ProductDTO
                {
                    CategoryName = SD.Bracelets,
                    Title = worksheet.Cells[row, 5].Text,
                    EventName = worksheet.Cells[row, 5].Text,
                    VenderName = worksheet.Cells[row, 6].Text,
                    VenderStyle = worksheet.Cells[row, 7].Text,
                    Sku = worksheet.Cells[row, 7].Text,
                    Length = worksheet.Cells[row, 9].Text,
                    GoldWeight = worksheet.Cells[row, 11].Text,
                    CTW = worksheet.Cells[row, 12].Text,
                    ColorName = worksheet.Cells[row, 15].Text,
                    AccentStoneShapeName = worksheet.Cells[row, 17].Text,
                    MMSize = worksheet.Cells[row, 18].Text,
                    DiaWT = decimal.TryParse(worksheet.Cells[row, 19].Text, out var diaWt) ? diaWt : 0,
                    NoOfStones = int.TryParse(worksheet.Cells[row, 20].Text, out var noOfStones) ? noOfStones : 0,
                    Grades = worksheet.Cells[row, 21].Text,
                    ProductType = worksheet.Cells[row, 22].Text,
                    Price = ConvertStringToDecimal(worksheet.Cells[row, 23].Text),
                    UnitPrice = ConvertStringToDecimal(worksheet.Cells[row, 23].Text),
                    WholesaleCost = decimal.TryParse(worksheet.Cells[row, 26].Text, out var hCost) ? hCost : 0,
                    Description = worksheet.Cells[row, 32].Text,
                    IsReadyforShip = !string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text) && worksheet.Cells[row, 1].Text == SD.ReadyToPdp ? true : false
                };

                if (!string.IsNullOrWhiteSpace(product.EventName) && !string.IsNullOrWhiteSpace(product.VenderName) && !string.IsNullOrWhiteSpace(product.VenderStyle) && !string.IsNullOrWhiteSpace(product.Sku))
                {
                    tempProducts = new ProductDTO();
                    tempProducts = product;
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);
                }
                else
                {
                    product.CategoryName = SD.Bracelets;
                    product.EventName = string.IsNullOrWhiteSpace(product.EventName) ? tempProducts.EventName : product.EventName;
                    product.Title = string.IsNullOrWhiteSpace(product.EventName) ? tempProducts.EventName : product.EventName;
                    product.VenderName = string.IsNullOrWhiteSpace(product.VenderName) ? tempProducts.VenderName : product.VenderName;
                    product.VenderStyle = string.IsNullOrWhiteSpace(product.VenderStyle) ? tempProducts.VenderStyle : product.VenderStyle;
                    product.Sku = string.IsNullOrWhiteSpace(product.Sku) ? tempProducts.Sku : product.Sku;
                    product.Length = string.IsNullOrWhiteSpace(product.Length) ? tempProducts.Length : product.Length;
                    product.GoldWeight = string.IsNullOrWhiteSpace(product.GoldWeight) ? tempProducts.GoldWeight : product.GoldWeight;
                    product.CTW = string.IsNullOrWhiteSpace(product.CTW) ? tempProducts.CTW : product.CTW;
                    product.ColorName = string.IsNullOrWhiteSpace(product.ColorName) ? tempProducts.ColorName : product.ColorName;
                    product.AccentStoneShapeName = string.IsNullOrWhiteSpace(product.AccentStoneShapeName) ? tempProducts.AccentStoneShapeName : product.AccentStoneShapeName;
                    product.MMSize = string.IsNullOrWhiteSpace(product.MMSize) ? tempProducts.MMSize : product.MMSize;
                    product.DiaWT = product.DiaWT == 0 ? tempProducts.DiaWT : product.DiaWT;
                    product.NoOfStones = product.NoOfStones == 0 ? tempProducts.NoOfStones : product.NoOfStones;
                    product.Grades = string.IsNullOrWhiteSpace(product.Grades) ? tempProducts.Grades : product.Grades;
                    product.ProductType = string.IsNullOrWhiteSpace(product.ProductType) ? tempProducts.ProductType : product.ProductType;
                    product.Price = product.Price == 0 ? tempProducts.Price : product.Price;
                    product.UnitPrice = product.Price == 0 ? tempProducts.UnitPrice : product.UnitPrice;
                    product.Description = string.IsNullOrWhiteSpace(product.Description) ? tempProducts.Description : product.Description;
                    product.WholesaleCost = product.WholesaleCost > 0 ? tempProducts.WholesaleCost : product.WholesaleCost;
                    product.IsReadyforShip = product.IsReadyforShip != true ? tempProducts.IsReadyforShip : product.IsReadyforShip;
                    products.Add(product);
                    newProductList = CreateRingVariantsFromColor(product);
                    products.AddRange(newProductList);

                }
            }

            return products;
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> RequestedProductList()
        {
            var productList = await _productRepository.GetProductPendingList();
            return View(productList);
        }

        [HttpGet]
        public async Task<IActionResult> HoldProductList()
        {
            var productList = await _productRepository.GetProductHoldList();
            return View(productList);
        }

        [HttpGet]
        public async Task<IActionResult> DeactivatedProductList()
        {
            var productList = await _productRepository.GetProductDeActivatedList();
            return View(productList);
        }


        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string[] pIds, string status)
        {
            if (pIds == null || pIds.Length == 0 || string.IsNullOrWhiteSpace(status))
            {
                return Json("Invalid input: Product IDs and status are required.");
            }

            bool isUpdated = await _productRepository.UpdateProductStatus(pIds, status);

            string message = isUpdated
                ? "Product status has been successfully updated."
                : "Failed to update product status. Please try again.";

            return Json(message);
        }

        [HttpGet]
        public async Task<IActionResult> JewelryProperty()
        {
            var propertyList = await _productPropertyRepository.GetMainPropertyList();
            return View(propertyList);
        }

        [HttpGet]
        public async Task<IActionResult> JewelryPropertyItems(string propertyName)
        {
            var jewelryPropertyList = await _productPropertyRepository.GetPropertyItemsByName(propertyName);
            return PartialView("~/Views/Jewellery/_ProductPropertyItems.cshtml", jewelryPropertyList);
        }


        [HttpGet]
        public async Task<IActionResult> UpsertProductProperty(int? pId = 0)
        {
            ViewBag.ParentDrp = await _productPropertyRepository.GetMainPropertyList();
            var isEdit = pId.HasValue && pId > 0;
            ViewBag.Title = isEdit ? "Edit Jewellery Property" : "Create Jewellery Property";

            var data = isEdit
                ? await _productPropertyRepository.GetProductPropertyById(pId.Value)
                : new ProductProperty();

            return View(data);
        }


        [HttpPost]
        public async Task<IActionResult> UpsertProductProperty(ProductProperty productPropData, IFormFile UploadFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentDrp = await _productPropertyRepository.GetMainPropertyList();
                return View(productPropData);
            }

            // Handle file upload
            if (UploadFile != null && UploadFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Ensure the folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(UploadFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadFile.CopyToAsync(stream);
                }

                // Optionally assign file name to productPropData for DB storage
                productPropData.IconPath = "/uploads/" + uniqueFileName;
            }

            await _productPropertyRepository.SaveProductProperty(productPropData, productPropData.Id);

            TempData["Status"] = "Success";
            TempData["Message"] = "Product property has been updated successfully.";

            return RedirectToAction("JewelryProperty");
        }


        [HttpGet]
        public async Task<IActionResult> DeleteProductProperty(int? pId = 0)
        {
            if (!pId.HasValue || pId <= 0)
            {
                TempData["Status"] = "Fail";
                TempData["Message"] = "Invalid property ID.";
                return RedirectToAction("JewelryProperty");
            }

            var success = await _productPropertyRepository.DeleteProductProperty(pId.Value);

            if (!success)
            {
                TempData["Status"] = "Fail";
                TempData["Message"] = "Property not found or could not be deleted.";
                return RedirectToAction("JewelryProperty");
            }

            TempData["Status"] = "Success";
            TempData["Message"] = "Property has been saved successfully.";
            return RedirectToAction("JewelryProperty");
        }

        private decimal ConvertStringToDecimal(string CellValue)
        {
            // Remove $ sign and commas
            string cleaned = CellValue.Replace("$", "").Replace(",", "");

            if (decimal.TryParse(cleaned, out decimal result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        private List<ProductDTO> CreateRingVariantsFromColor(ProductDTO baseProduct)
        {
            var result = new List<ProductDTO>();

            int lastSpaceIndex = baseProduct.ColorName?.LastIndexOf(' ') ?? -1;
            if (lastSpaceIndex == -1) return result;

            string leftPart = baseProduct.ColorName[..lastSpaceIndex];
            string productType = baseProduct.ColorName[(lastSpaceIndex + 1)..];

            var leftParts = leftPart.Split(' ', 2);
            if (leftParts.Length < 2) return result;

            string karat = leftParts[0];
            var metals = leftParts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var metal in metals)
            {
                result.Add(new ProductDTO
                {
                    Id = baseProduct.Id,
                    Title = baseProduct.Title,
                    CategoryName = baseProduct.CategoryName,
                    VenderName = baseProduct.VenderName,
                    VenderStyle = baseProduct.VenderStyle,
                    Sku = baseProduct.Sku,
                    Length = baseProduct.Length,
                    BandWidth = baseProduct.BandWidth,
                    GoldWeight = baseProduct.GoldWeight,
                    CTW = baseProduct.CTW,
                    CenterCaratName = baseProduct.CenterCaratName,
                    CenterShapeName = baseProduct.CenterShapeName,
                    Grades = baseProduct.Grades,
                    DiaWT = baseProduct.DiaWT,
                    NoOfStones = baseProduct.NoOfStones,
                    Quantity = baseProduct.NoOfStones,
                    Price = baseProduct.Price,
                    UnitPrice = baseProduct.UnitPrice,
                    ColorName = metal.Trim(),
                    Karat = karat,
                    ProductType = baseProduct.ProductType,
                    MMSize = baseProduct.MMSize,
                    IsActivated = true,
                    Certificate = baseProduct.Certificate,
                    AccentStoneShapeName = baseProduct.AccentStoneShapeName,
                    Description = baseProduct.Description,
                    IsReadyforShip = baseProduct.IsReadyforShip,
                    WholesaleCost = baseProduct.WholesaleCost,
                    Diameter = baseProduct.Diameter
                });
            }
            return result;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadProductCollectionImages()
        {
            try
            {
                var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;
                var reader = new MultipartReader(boundary, Request.Body);
                var section = await reader.ReadNextSectionAsync();

                if (section == null)
                    return Json("No file uploaded.");

                while (section != null)
                {
                    if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                    {
                        if (contentDisposition.DispositionType == "form-data" &&
                            !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                        {
                            string fileName = contentDisposition.FileName.Value.Trim('"');
                            string extractedFolder = Path.Combine(_env.WebRootPath, "UploadedFiles", "Collections");
                            Directory.CreateDirectory(extractedFolder);

                            string zipPath = Path.Combine(extractedFolder, fileName);
                            await using (var targetStream = System.IO.File.Create(zipPath))
                            {
                                await section.Body.CopyToAsync(targetStream);
                            }

                            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                            {
                                foreach (ZipArchiveEntry entry in archive.Entries)
                                {
                                    if (entry.FullName.EndsWith("/") || string.IsNullOrEmpty(entry.Name)) continue;

                                    if (entry.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                        entry.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                        entry.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                        entry.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                                    {
                                        var styleName = _productRepository.ExtractStyleName(entry.Name);
                                        if (styleName == null) continue;

                                        var metalId = await _productRepository.GetMetalId(styleName.ColorName);
                                        if (metalId == 0) continue;

                                        var prdDesignDT = await _productRepository.GetProductDataByDesignNo(styleName.DesignNo, metalId);
                                        if (prdDesignDT.Count == 0) continue;

                                        string folderPath = Path.Combine(extractedFolder, styleName.DesignNo);
                                        Directory.CreateDirectory(folderPath);

                                        string destinationPath = Path.Combine(folderPath, entry.Name);
                                        entry.ExtractToFile(destinationPath, overwrite: true);

                                        string relativePath = Path.Combine("UploadedFiles", "Collections", styleName.DesignNo, entry.Name).Replace("\\", "/");

                                        foreach (var pro in prdDesignDT)
                                        {
                                            var prdDT = new ProductImages
                                            {
                                                ProductId = pro.Id.ToString(),
                                                MetalId = metalId,
                                                Sku = styleName.DesignNo,
                                                ShapeId = pro.ShapeId,
                                                ImageIndexNumber = styleName.Index,
                                                IsDefault = (styleName.Index == 1)
                                            };

                                            if (entry.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                                            {
                                                prdDT.VideoId = await _productRepository.SaveImageVideoPath(relativePath);
                                            }
                                            else
                                            {
                                                // Save original (Lg)
                                                int lgId = await _productRepository.SaveImageVideoPath(relativePath);
                                                prdDT.ImageLgId = lgId;

                                                using (var stream = entry.Open())
                                                using (var ms = new MemoryStream())
                                                {
                                                    await stream.CopyToAsync(ms);
                                                    ms.Position = 0;

                                                    using var originalImage = Image.FromStream(ms);

                                                    var sizes = new Dictionary<string, Size>
                                                    {
                                                        { "Md", new Size(500, 500) },
                                                        { "Sm", new Size(200, 200) }
                                                    };

                                                    foreach (var size in sizes)
                                                    {
                                                        string resizedName = $"{Path.GetFileNameWithoutExtension(entry.Name)}_{size.Key}{Path.GetExtension(entry.Name)}";
                                                        string resizedPath = Path.Combine(folderPath, resizedName);

                                                        ResizeAndSaveImage(originalImage, resizedPath, size.Value);

                                                        string resizedRelativePath = Path.Combine("UploadedFiles", "Collections", styleName.DesignNo, resizedName)
                                                                                        .Replace("\\", "/");

                                                        int resizedId = await _productRepository.SaveImageVideoPath(resizedRelativePath);

                                                        switch (size.Key)
                                                        {
                                                            case "Md":
                                                                prdDT.ImageMdId = resizedId;
                                                                break;
                                                            case "Sm":
                                                                prdDT.ImageSmId = resizedId;
                                                                break;
                                                        }
                                                    }
                                                }
                                            }

                                            await _productRepository.SaveImageVideoAsync(prdDT);
                                        }
                                    }
                                }
                            }

                            System.IO.File.Delete(zipPath); // Clean up
                        }
                    }

                    section = await reader.ReadNextSectionAsync();
                }

                return Json("Images and Videoes have been uploaded successfully.");

            }
            catch (Exception ex)
            {
                return Json("Images and Videoes have been failed to upload.");
            }
        }


        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 5368709120)]
        [RequestSizeLimit(5368709120)]
        public async Task<IActionResult> UploadProductImagesFromFolder([FromForm] List<IFormFile> folderUpload)
        {
            try
            {
                if (folderUpload == null || folderUpload.Count == 0)
                    return Json(new { success = false, message = "No files uploaded." });

                // Get the encryption key
                var keyFile = Request.Form.Files["encryptionKey"];
                if (keyFile == null)
                    return Json(new { success = false, message = "Encryption key is missing." });

                byte[] keyBytes;
                using (var ms = new MemoryStream())
                {
                    await keyFile.CopyToAsync(ms);
                    keyBytes = ms.ToArray();
                }

                foreach (var file in folderUpload)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var styleName = _productRepository.ExtractStyleName(fileName);
                    if (styleName.ColorName == null && styleName.DesignNo == null) continue;

                    var metalId = await _productRepository.GetMetalId(styleName.ColorName);
                    if (metalId == 0) continue;

                    var prdDesignDT = await _productRepository.GetProductDataByDesignNo(styleName.DesignNo, metalId);
                    if (prdDesignDT.Count == 0) continue;

                    // Decrypt the file content
                    byte[] encryptedBytes;
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        encryptedBytes = ms.ToArray();
                    }

                    byte[] decryptedBytes;
                    try
                    {
                        decryptedBytes = DecryptAesGcm(encryptedBytes, keyBytes);
                    }
                    catch (Exception)
                    {
                        // Skip file if decryption fails
                        continue;
                    }

                    foreach (var pro in prdDesignDT)
                    {
                        var prdDT = new ProductImages
                        {
                            ProductId = pro.Id.ToString(),
                            MetalId = metalId,
                            Sku = styleName.DesignNo,
                            ShapeId = pro.ShapeId
                        };

                        string baseFolder = Path.Combine(_env.WebRootPath, "UploadedFiles", "Collections", styleName.DesignNo);
                        Directory.CreateDirectory(baseFolder);

                        if (fileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                        {
                            string destPath = Path.Combine(baseFolder, fileName);
                            await System.IO.File.WriteAllBytesAsync(destPath, decryptedBytes);

                            string relativePath = Path.Combine("UploadedFiles", "Collections", styleName.DesignNo, fileName).Replace("\\", "/");
                            prdDT.VideoId = await _productRepository.SaveImageVideoPath(relativePath);
                        }
                        else
                        {
                            using var ms = new MemoryStream(decryptedBytes);
                            string lgFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_Lg{Path.GetExtension(fileName)}";
                            string lgPath = Path.Combine(baseFolder, lgFileName);

                            await using (var fs = new FileStream(lgPath, FileMode.Create))
                            {
                                ms.Position = 0;
                                await ms.CopyToAsync(fs);
                            }

                            string lgRelativePath = Path.Combine("UploadedFiles", "Collections", styleName.DesignNo, lgFileName).Replace("\\", "/");
                            prdDT.ImageLgId = await _productRepository.SaveImageVideoPath(lgRelativePath);

                            ms.Position = 0;
                            using var originalImage = Image.FromStream(ms);

                            var sizes = new Dictionary<string, Size>
                            {
                                { "Md", new Size(500, 500) },
                                { "Sm", new Size(200, 200) }
                            };

                            foreach (var size in sizes)
                            {
                                string resizedFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{size.Key}{Path.GetExtension(fileName)}";
                                string resizedPath = Path.Combine(baseFolder, resizedFileName);

                                ResizeAndSaveImage(originalImage, resizedPath, size.Value);

                                string relativePath = Path.Combine("UploadedFiles", "Collections", styleName.DesignNo, resizedFileName).Replace("\\", "/");
                                int fileId = await _productRepository.SaveImageVideoPath(relativePath);

                                switch (size.Key)
                                {
                                    case "Md":
                                        prdDT.ImageMdId = fileId;
                                        break;
                                    case "Sm":
                                        prdDT.ImageSmId = fileId;
                                        break;
                                }
                            }
                        }

                        await _productRepository.SaveImageVideoAsync(prdDT);
                    }
                }

                return Json(new { success = true, message = "Files uploaded and processed successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        private void ResizeAndSaveImage(Image originalImage, string outputPath, Size targetSize)
        {
            int width, height;
            float ratio = Math.Min((float)targetSize.Width / originalImage.Width, (float)targetSize.Height / originalImage.Height);
            width = (int)(originalImage.Width * ratio);
            height = (int)(originalImage.Height * ratio);

            using var resized = new Bitmap(targetSize.Width, targetSize.Height);
            using (var g = Graphics.FromImage(resized))
            {
                g.Clear(Color.White); // Optional: background color
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Draw image centered
                int x = (targetSize.Width - width) / 2;
                int y = (targetSize.Height - height) / 2;
                g.DrawImage(originalImage, x, y, width, height);
            }

            resized.Save(outputPath, ImageFormat.Jpeg);
        }

        private byte[] DecryptAesGcm(byte[] encryptedData, byte[] key)
        {
            const int IvSize = 12;
            const int TagSize = 16;

            if (encryptedData.Length < IvSize + TagSize)
                throw new Exception("Invalid encrypted data length.");

            byte[] iv = encryptedData.Take(IvSize).ToArray();
            byte[] ciphertext = encryptedData.Skip(IvSize).Take(encryptedData.Length - IvSize - TagSize).ToArray();
            byte[] tag = encryptedData.Skip(encryptedData.Length - TagSize).ToArray();

            byte[] plaintext = new byte[ciphertext.Length];

            using var aesGcm = new AesGcm(key);
            aesGcm.Decrypt(iv, ciphertext, tag, plaintext);

            return plaintext;
        }


        [HttpGet]
        public IActionResult ThankYouForUploaded()
        {
            return View();
        }

    }
}
