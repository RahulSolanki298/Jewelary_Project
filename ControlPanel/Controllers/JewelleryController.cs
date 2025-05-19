using Business.Repository.IRepository;
using Common;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var productList= await _productRepository.GetProductStyleList();
            return PartialView("_JewelleryList", productList);
        }

        [HttpGet]
        public async Task<IActionResult> UploadJewellery()
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
        public async Task<IActionResult> SaveAllProduct(List<ProductDTO> products)
        {
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
                else if (wkSheet.CategoryName.Trim().ToLower() == "wedding bands")
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
                    EventName = worksheet.Cells[row, 5].Text,
                    Title = worksheet.Cells[row, 5].Text,
                    VenderName = worksheet.Cells[row, 6].Text,
                    VenderStyle = worksheet.Cells[row, 7].Text,
                    Sku = worksheet.Cells[row, 7].Text,
                    Length = worksheet.Cells[row, 9].Text,
                    BandWidth = worksheet.Cells[row, 10].Text,
                    GoldWeight = worksheet.Cells[row, 11].Text,
                    CTW = worksheet.Cells[row, 12].Text,
                    CenterShapeName = worksheet.Cells[row, 13].Text,
                    CenterCaratName = worksheet.Cells[row, 14].Text,
                    Certificate = worksheet.Cells[row, 15].Text,
                    ColorName = worksheet.Cells[row, 16].Text,
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
                    product.EventName = string.IsNullOrWhiteSpace(product.EventName) ? tempProducts.EventName : product.EventName;
                    product.Title = string.IsNullOrWhiteSpace(product.EventName) ? tempProducts.EventName : product.EventName;
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
                    product.Title = string.IsNullOrWhiteSpace(product.EventName) ? tempProducts.EventName : product.EventName;
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

        private List<ProductDTO> ProcessBraceletsData(ExcelWorksheet worksheet)
        {
            var newProductList = new List<ProductDTO>();
            var products = new List<ProductDTO>();
            var tempProducts = new ProductDTO();
            int rowCount = worksheet.Dimension.Rows;
            ProductDTO product = null;
            var titValue = worksheet.Cells[4, 3].Text;
            for (int row = 5; row <= rowCount; row++)
            {
                product = new ProductDTO
                {
                    CategoryName = SD.Bracelets,
                    Title = titValue,
                    EventName = titValue,
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
                    Description = worksheet.Cells[row, 31].Text,
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
                    Title=baseProduct.Title,
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
                    ProductType = productType,
                    MMSize = baseProduct.MMSize,
                    IsActivated = true,
                    EventName = baseProduct.EventName,
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
    }
}
