using Business.Repository.IRepository;
using Common;
using DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using Stripe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductPropertyRepository _productPropertyRepository;
        private readonly IWebHostEnvironment _env;
        public ProductController(IProductRepository productRepository, 
            IProductPropertyRepository productPropertyRepository,
            IWebHostEnvironment env)
        {
            _productRepository = productRepository;
            _productPropertyRepository = productPropertyRepository;
            _env=env;
        }

        #region Useful APIS

        [HttpPost("BulkNewProductCollectionUpload")]
        public async Task<IActionResult> UploadNewProductCollectionExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Ensure the file is an Excel file
                if (!file.FileName.EndsWith(".xlsx"))
                {
                    return BadRequest("Invalid file format. Please upload an Excel (.xlsx) or (.csv) file.");
                }

                // Read the file using EPPlus
                using (var stream = new MemoryStream())
                {

                    await file.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var workbook = package.Workbook;

                        var worksheet = workbook.Worksheets[0]; // Assuming you want the first sheet

                        var rows = new List<Models.ProductDTO>();

                        // Loop through rows and columns
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++) // Start at row 2 to skip header
                        {
                            var date = worksheet.Cells[row, 2].Value;
                            var itemDate = ParseDate(date);

                            var data = new ProductDTO
                            {
                                DesignNo = worksheet.Cells[row, 1].Text,
                                ProductDate = itemDate ?? DateTime.MinValue,
                                ParentDesign = worksheet.Cells[row, 3].Text,
                                CaratName = worksheet.Cells[row, 4].Text,
                                Gender = worksheet.Cells[row, 5].Text,
                                CollectionName = worksheet.Cells[row, 6].Text,
                                CategoryName = worksheet.Cells[row, 7].Text,
                                SubCategoryName = worksheet.Cells[row, 8].Text,
                                ProductType = worksheet.Cells[row, 9].Text,
                                StyleName = worksheet.Cells[row, 10].Text,
                                Occasion = worksheet.Cells[row, 11].Text,
                                Remarks = worksheet.Cells[row, 12].Text,
                                Package = worksheet.Cells[row, 13].Text,
                                MfgDesign = worksheet.Cells[row, 14].Text,
                                VenderName = worksheet.Cells[row, 15].Text,
                                Title = worksheet.Cells[row, 16].Text,
                                Designer = worksheet.Cells[row, 17].Text,
                                CadDesigner = worksheet.Cells[row, 18].Text,
                                Id = Guid.NewGuid()
                            };

                            rows.Add(data);
                        }


                        await _productRepository.SaveNewProductCollectionList(rows);

                    }
                }

                return Ok("File uploaded and processed successfully.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("UpdateProductDataByExcel")]
        public async Task<IActionResult> UpdateProductDataByExcel(IFormFile file)
        {
            var data = new ProductDTO();
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Ensure the file is an Excel file
                if (!file.FileName.EndsWith(".xlsx"))
                {
                    return BadRequest("Invalid file format. Please upload an Excel (.xlsx) or (.csv) file.");
                }

                // Read the file using EPPlus
                using (var stream = new MemoryStream())
                {

                    await file.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var workbook = package.Workbook;

                        var worksheet = workbook.Worksheets[0]; // Assuming you want the first sheet

                        var rows = new List<Models.ProductDTO>();

                        // Loop through rows and columns
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++) // Start at row 2 to skip header
                        {
                            //var date = worksheet.Cells[row, 2].Value;
                            //var itemDate = ParseDate(date);

                            data = new ProductDTO();

                            data.DesignNo = worksheet.Cells[row, 1].Text;
                            data.ShapeName = string.IsNullOrEmpty(worksheet.Cells[row, 5].Text) != true ? worksheet.Cells[row, 5].Text : "";
                            data.ClarityName = string.IsNullOrEmpty(worksheet.Cells[row, 6].Text) != true && worksheet.Cells[row, 4].Text == "LGD"
                                            ? worksheet.Cells[row, 6].Text : "-";
                            data.Karat = string.IsNullOrEmpty(worksheet.Cells[row, 6].Text) != true && worksheet.Cells[row, 4].Text == "GOLD"
                                            ? worksheet.Cells[row, 6].Text : "-";
                            data.ColorName = worksheet.Cells[row, 7].Text;
                            data.Carat = worksheet.Cells[row, 8].Text;
                            data.Quantity = string.IsNullOrEmpty(worksheet.Cells[row, 10].Text) != true ?
                                             Convert.ToInt32(worksheet.Cells[row, 10].Text) : 0;
                            //data.DiaWT = string.IsNullOrEmpty(worksheet.Cells[row, 11].Text) != true ? Convert.ToInt32(worksheet.Cells[row, 11].Text) : 0;
                            data.ProductType = worksheet.Cells[row, 4].Text;
                            data.Setting = worksheet.Cells[row, 9].Text;
                            data.Id = Guid.NewGuid();


                            rows.Add(data);
                        }


                        await _productRepository.UpdateProductDetailsByExcel(rows);

                    }
                }

                return Ok("File uploaded and processed successfully.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


         [HttpPost("BulkProductCollectionImagesUpload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 5368709120)]
        [RequestSizeLimit(5368709120)]
        public async Task<IActionResult> UploadProductCollectionImages([FromForm] IFormFile zipFile)
        {
            try
            {
                List<DataAccess.Entities.Product> prdDesignDT = new List<DataAccess.Entities.Product>();
                var prdDT = new ProductImages();
                if (zipFile == null || zipFile.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                string extractedFolder = Path.Combine(_env.WebRootPath, "UploadedFiles", "Collections");
                Directory.CreateDirectory(extractedFolder);

                string zipPath = Path.Combine(extractedFolder, zipFile.FileName);
                using (var fileStream = new FileStream(zipPath, FileMode.Create))
                {
                    await zipFile.CopyToAsync(fileStream);
                }

                var productImages = new List<ProductImages>();

                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            entry.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                            entry.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                            entry.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                        {
                            var styleName = _productRepository.ExtractStyleName(entry.Name);
                            if (styleName == null) continue;  

                            var metalId = await _productRepository.GetMetalId(styleName.ColorName);
                            if (metalId == 0) continue;

                            prdDesignDT = await _productRepository.GetProductDataByDesignNo(styleName.DesignNo, metalId);
                            if (prdDesignDT.Count == 0) continue;

                            foreach (var pro in prdDesignDT)
                            {
                                prdDT = new ProductImages
                                {
                                    ProductId = pro.Id.ToString(),
                                    MetalId = metalId,
                                    Sku = styleName.DesignNo,
                                    ShapeId = pro.ShapeId
                                };

                                string folderPath = Path.Combine(extractedFolder, styleName.DesignNo);
                                if (!Directory.Exists(folderPath))
                                {
                                    Directory.CreateDirectory(folderPath);
                                }

                                string destinationPath = Path.Combine(folderPath, entry.Name);
                                entry.ExtractToFile(destinationPath, overwrite: true);
                                string relativePath = Path.Combine("UploadedFiles", "Collections", styleName.DesignNo, entry.Name).Replace("\\", "/");

                                int pId = await _productRepository.SaveImageVideoPath(relativePath);

                                if (entry.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                                {
                                    prdDT.VideoId = pId;
                                }
                                else
                                {
                                    if (styleName.Index == 1)
                                    {
                                        prdDT.ImageLgId = pId;
                                        prdDT.IsDefault = true;
                                    }
                                    else
                                    {
                                        prdDT.ImageLgId = pId;
                                    }
                                    prdDT.ImageIndexNumber = styleName.Index;
                                }

                                await _productRepository.SaveImageVideoAsync(prdDT);
                            }
                        }
                    }
                }

                if (System.IO.File.Exists(zipPath))
                {
                    System.IO.File.Delete(zipPath);
                }

                return Ok("File Uploaded Successfully.");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("BulkProductImagesUploadFromFolder")]
        public async Task<IActionResult> UploadProductImagesFromFolder([FromForm] string folderPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                {
                    return BadRequest("Folder path is invalid or doesn't exist.");
                }

                var productImages = new List<ProductImages>();
                var allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                                        .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                    f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                    f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                    f.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase));

                foreach (var file in allFiles)
                {
                    var fileName = Path.GetFileName(file);

                    var styleName = _productRepository.ExtractStyleName(fileName);
                    if (styleName == null) continue;

                    var metalId = await _productRepository.GetMetalId(styleName.ColorName);
                    if (metalId == 0) continue;

                    var prdDesignDT = await _productRepository.GetProductDataByDesignNo(styleName.DesignNo, metalId);
                    if (prdDesignDT.Count == 0) continue;

                    foreach (var pro in prdDesignDT)
                    {
                        var prdDT = new ProductImages
                        {
                            ProductId = pro.Id.ToString(),
                            MetalId = metalId,
                            Sku = styleName.DesignNo,
                            ShapeId = pro.ShapeId
                        };

                        // Copy image to structured folder
                        string destFolder = Path.Combine(_env.WebRootPath, "UploadedFiles", "Collections", styleName.DesignNo);
                        Directory.CreateDirectory(destFolder);

                        string destPath = Path.Combine(destFolder, fileName);
                        System.IO.File.Copy(file, destPath, overwrite: true);

                        string relativePath = Path.Combine("UploadedFiles", "Collections", styleName.DesignNo, fileName).Replace("\\", "/");
                        int pId = await _productRepository.SaveImageVideoPath(relativePath);

                        if (fileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                        {
                            prdDT.VideoId = pId;
                        }
                        else
                        {
                            prdDT.ImageLgId = pId;
                            prdDT.IsDefault = styleName.Index == 1;
                            prdDT.ImageIndexNumber = styleName.Index;
                        }

                        await _productRepository.SaveImageVideoAsync(prdDT);
                    }
                }

                return Ok("Images processed and uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost("ExcelUploadForProduct")]
        public async Task<IActionResult> ExcelUpload(IFormFile file)
        {
            List<ProductDTO> ProductUpload = new List<ProductDTO>();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid file format. Please upload an Excel (.xlsx) file.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);

            foreach (var wkSheet in package.Workbook.Worksheets)
            {

                if (wkSheet.Name.Trim().ToLower() == "engagement rings")
                {
                    var ringProducts = ProcessRingData(wkSheet);
                    ProductUpload.AddRange(ringProducts);
                }
                else if (wkSheet.Name.Trim().ToLower() == "wedding bands")
                {
                    var weddings = ProcessBandsData(wkSheet);
                    ProductUpload.AddRange(weddings);
                }
                else if (wkSheet.Name.Trim().ToLower() == "earrings")
                {
                    var weddings = ProcessEarringsData(wkSheet);
                    ProductUpload.AddRange(weddings);
                }
                else if (wkSheet.Name.Trim().ToLower() == "pendants")
                {
                    var weddings = ProcessPendantsData(wkSheet);
                    ProductUpload.AddRange(weddings);
                }
                else if (wkSheet.Name.Trim().ToLower() == "bracelets")
                {
                    var weddings = ProcessBraceletsData(wkSheet);
                    ProductUpload.AddRange(weddings);
                }
            }

            return Ok(ProductUpload);

        }


        [HttpPost("BulkNewProductUpload")]
        public async Task<IActionResult> UploadNewExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid file format. Please upload an Excel (.xlsx) file.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);

            foreach (var wkSheet in package.Workbook.Worksheets)
            {

                if (wkSheet.Name.Trim().ToLower() == "engagement rings")
                {
                    var ringProducts = ProcessRingData(wkSheet);
                    await _productRepository.SaveNewProductList(ringProducts, "Rings");
                }
                else if (wkSheet.Name.Trim().ToLower() == "wedding bands")
                {
                    var weddings = ProcessBandsData(wkSheet);
                    await _productRepository.SaveNewProductList(weddings, "Bands");
                }
                else if (wkSheet.Name.Trim().ToLower() == "earrings")
                {
                    var weddings = ProcessEarringsData(wkSheet);
                    await _productRepository.SaveEarringsList(weddings, "Earrings");
                }
                else if (wkSheet.Name.Trim().ToLower() == "pendants")
                {
                    var weddings = ProcessPendantsData(wkSheet);
                    await _productRepository.SaveEarringsList(weddings, "Pendants"); // Hypothetical method
                }
                else if (wkSheet.Name.Trim().ToLower() == "bracelets")
                {
                    var weddings = ProcessBraceletsData(wkSheet);
                    await _productRepository.SaveEarringsList(weddings, "Bracelets"); // Hypothetical method
                }
            }

            return Ok("Data Uploaded Successfully.");

        }


        [HttpPost("SaveAllProduct")]
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

            return Ok("Data Uploaded Successfully.");

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


        [HttpPost("GetProductsByFilters")]
        public async Task<IActionResult> GetProductsByFilters(ProductFilters filters, int pageNumber = 1, int pageSize = 10)
        {
            var products = await _productRepository.GetProductStyleList();
            var query = products.AsQueryable();

            var shapeIds = filters.Shapes?.Select(Int32.Parse).ToList();
            var metalIds = filters.Metals?.Select(Int32.Parse).ToList();

            if (shapeIds?.Any() == true)
            {
                query = query.Where(p => p.Shapes.Any(shape => shapeIds.Contains(p.ShapeId.Value)));
            }

            if (metalIds?.Any() == true)
            {
                query = query.Where(p => p.Metals.Any(metal => metalIds.Contains(p.ColorId.Value)));
            }

            if (filters.FromPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filters.FromPrice.Value);
            }

            if (filters.ToPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filters.ToPrice.Value);
            }

            if (filters.FromCarat.HasValue)
            {
                query = query.Where(p => Convert.ToDecimal(p.CenterCaratName) >= filters.FromCarat.Value);
            }

            if (filters.ToCarat.HasValue)
            {
                query = query.Where(p => Convert.ToDecimal(p.CenterCaratName) <= filters.ToCarat.Value);
            }

            if (filters.categories != null  && filters.categories.Length > 0 && filters.categories[0] != null)
            {
                query = query.Where(p => filters.categories.Contains(p.CategoryName));
            }

            if (!string.IsNullOrEmpty(filters.OrderBy))
            {
                switch (filters.OrderBy.ToLower())
                {
                    case "asc":
                        query = query.OrderBy(p => p.Title); // assuming product has a Name property
                        break;
                    case "desc":
                        query = query.OrderByDescending(p => p.Title);
                        break;
                    case "price":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "priceMax":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                }
            }

            // Pagination
            var pagedResult = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(pagedResult);
        }


        [HttpGet("GetProductDetails/{productId}")]
        public async Task<IActionResult> GetProductsByFilters(string productId)
        {
            var products = await _productRepository.GetProductWithDetails(productId);
            return Ok(products);

        }


        [HttpGet("GetProductByColor/Sku/{sku}/colorId/{colorId}")]
        public async Task<IActionResult> GetProductsByColorId(string sku, int colorId)
        {
            var products = await _productRepository.GetProductByColorId(sku, colorId, 0);
            return Ok(products);
        }


        [HttpGet("GetProductByCarat/Sku/{sku}/caratId/{caratId}")]
        public async Task<IActionResult> GetProductsByCaratId(string sku, int caratId)
        {
            var products = await _productRepository.GetProductByColorId(sku, 0, caratId);
            return Ok(products);

        }

        #endregion

        #region Old APIs

        [HttpPost("BulkProductUpload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid file format. Please upload an Excel (.xlsx) file.");

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return BadRequest("The Excel file is empty.");

                int rowCount = worksheet.Dimension.Rows;
                List<ProductDTO> products = new();

                for (int row = 5; row <= rowCount; row++)
                {
                    var product = new ProductDTO
                    {
                        Id = Guid.NewGuid(),
                        CategoryName = worksheet.Name,
                        VenderName = worksheet.Cells[row, 3].Text,
                        StyleName = worksheet.Cells[row, 4].Text,
                        Sku = worksheet.Cells[row, 4].Text,
                        Length = worksheet.Cells[row, 5].Text,
                        GoldPurity = worksheet.Cells[row, 6].Text,
                        GoldWeight = worksheet.Cells[row, 7].Text,
                        CenterShapeName = worksheet.Cells[row, 9].Text,
                        CenterCaratName = worksheet.Cells[row, 10].Text,
                        ColorName = worksheet.Cells[row, 11].Text,
                        ShapeName = worksheet.Cells[row, 12].Text,
                        CaratSizeName = worksheet.Cells[row, 13].Text,
                        Grades = worksheet.Cells[row, 16].Text,
                        WebsiteImagesLink = worksheet.Cells[row, 18].Text,
                        CTW = worksheet.Cells[row, 8].Text
                    };

                    // Convert numeric values safely
                    product.DiaWT = decimal.TryParse(worksheet.Cells[row, 14].Text, out var diaWt) ? diaWt : 0;
                    product.NoOfStones = int.TryParse(worksheet.Cells[row, 15].Text, out var noOfStones) ? noOfStones : 0;
                    product.Price = string.IsNullOrEmpty(worksheet.Cells[row, 17].Text) != true ? Convert.ToDecimal(worksheet.Cells[row, 17].Text) : 0;
                    product.UnitPrice = product.Price;

                    // Handling multiple metal colors
                    var productMetals = product.ColorName.Split(",");
                    if (productMetals.Length > 0)
                    {
                        string carat = productMetals[0]; // Store carat separately
                        foreach (var metal in productMetals.Skip(1)) // Skip first item (carat)
                        {
                            products.Add(new ProductDTO
                            {
                                Id = product.Id,
                                CategoryName = product.CategoryName,
                                VenderName = product.VenderName,
                                StyleName = product.StyleName,
                                Sku = product.Sku,
                                Length = product.Length,
                                GoldPurity = product.GoldPurity,
                                GoldWeight = product.GoldWeight,
                                CTW = product.CTW,
                                CenterShapeName = product.CenterShapeName,
                                CenterCaratName = product.CenterCaratName,
                                CaratSizeName = product.CaratSizeName,
                                ShapeName = product.ShapeName,
                                Grades = product.Grades,
                                WebsiteImagesLink = product.WebsiteImagesLink,
                                DiaWT = product.DiaWT,
                                NoOfStones = product.NoOfStones,
                                Price = product.Price,
                                UnitPrice = product.UnitPrice,
                                CaratName = carat,
                                ColorName = metal.Trim(),
                                IsActivated = true
                            });
                        }
                    }
                }

                await _productRepository.SaveProductList(products);
                return Ok("File uploaded and processed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("BulkProductCollectionUpload")]
        public async Task<IActionResult> UploadProductCollectionExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Ensure the file is an Excel file
                if (!file.FileName.EndsWith(".xlsx"))
                {
                    return BadRequest("Invalid file format. Please upload an Excel (.xlsx) file.");
                }

                // Read the file using EPPlus
                using (var stream = new MemoryStream())
                {

                    await file.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var workbook = package.Workbook;

                        var worksheet = workbook.Worksheets[0]; // Assuming you want the first sheet

                        var rows = new List<Models.ProductDTO>();

                        // Loop through rows and columns
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++) // Start at row 2 to skip header
                        {
                            var data = new ProductDTO
                            {
                                DesignNo = worksheet.Cells[row, 1].Text,
                                GoldPurity = worksheet.Cells[row, 6].Text, // Column C
                                ShapeName = worksheet.Cells[row, 5].Text, // Column C
                                ColorName = worksheet.Cells[row, 7].Text,
                                CaratSizeName = worksheet.Cells[row, 8].Text,
                                GoldWeight = worksheet.Cells[row, 11].Text, // Column C
                                ClarityName = worksheet.Cells[row, 8].Text,
                                Quantity = Convert.ToInt32(worksheet.Cells[row, 12].Text)
                            };
                            rows.Add(data);
                        }

                        await _productRepository.SaveProductCollectionList(rows);

                    }
                }

                return Ok("File uploaded and processed successfully.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("BulkProductImagesUpload")]
        public async Task<IActionResult> UploadProductImages(IFormFile zipFile)
        {
            FileSplitDTO styles = new FileSplitDTO();
            if (zipFile == null || zipFile.Length == 0)
                return BadRequest("No file uploaded.");

            var extractedFolder = Path.Combine("UploadedFiles", "Styles");
            Directory.CreateDirectory(extractedFolder);

            var zipPath = Path.Combine(extractedFolder, zipFile.FileName);
            using (var fileStream = new FileStream(zipPath, FileMode.Create))
            {
                await zipFile.CopyToAsync(fileStream);
            }

            //ZipFile.ExtractToDirectory(zipPath, extractedFolder);

            //var files = Directory.GetDirectories(extractedFolder);
            List<string> savedImagePaths = new List<string>();

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        entry.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        entry.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        // Extract StyleName from image name
                        styles = _productRepository.ExtractStyleName(entry.Name);

                        // Create folder for StyleName
                        string folderPath = Path.Combine(extractedFolder, styles.DesignNo);
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        // Save extracted image
                        string destinationPath = Path.Combine(folderPath, entry.Name);
                        entry.ExtractToFile(destinationPath, overwrite: true);
                        savedImagePaths.Add(destinationPath);
                    }
                }
            }


            return Ok("Images uploaded and organized successfully.");
        }


        //private readonly AppDbContext _context;

        //public YourController(AppDbContext context)
        //{
        //    _context = context;
        //}

        // [HttpPost("BulkNewCollectionImagesUpload")]
        //public async Task<IActionResult> UploadNewProductCollectionImages(IFormFile zipFile)
        //{
        //    if (zipFile == null || zipFile.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    var extractedFolder = Path.Combine("UploadedFiles", "Collections");
        //    Directory.CreateDirectory(extractedFolder);

        //    var zipPath = Path.Combine(extractedFolder, zipFile.FileName);
        //    using (var fileStream = new FileStream(zipPath, FileMode.Create))
        //    {
        //        await zipFile.CopyToAsync(fileStream);
        //    }
        //    List<ProductImages> imageEntities = new List<ProductImages>();

        //    using (ZipArchive archive = ZipFile.OpenRead(zipPath))
        //    {
        //        foreach (ZipArchiveEntry entry in archive.Entries)
        //        {
        //            if (string.IsNullOrEmpty(entry.Name))
        //                continue;

        //            if (!entry.FullName.StartsWith("Images/", StringComparison.OrdinalIgnoreCase))
        //                continue;

        //            if (entry.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
        //                entry.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
        //                entry.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        //            {
        //                // Extract DesignNo and ColorCode from filename
        //                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(entry.Name);
        //                var parts = fileNameWithoutExt.Split('-');
        //                if (parts.Length < 2)
        //                    continue;

        //                string designNo = string.Join("-", parts.Take(parts.Length - 1));
        //                string colorCode = parts.Last();

        //                // Create folder for DesignNo
        //                string folderPath = Path.Combine(extractedFolder, designNo);
        //                Directory.CreateDirectory(folderPath);

        //                string destinationPath = Path.Combine(folderPath, entry.Name);
        //                entry.ExtractToFile(destinationPath, overwrite: true);

        //                // Save entity
        //                var imgUpload = new FileManager();
        //                imgUpload.FileUrl = destinationPath;

        //                var productImage = new ProductImages
        //                {
        //                    DesignNo = designNo,
        //                    ColorCode = colorCode,
        //                    ImagePath = destinationPath
        //                };

        //                imageEntities.Add(productImage);
        //            }
        //        }
        //    }

        //    if (imageEntities.Count > 0)
        //    {
        //        _context.ProductImages.AddRange(imageEntities);
        //        await _context.SaveChangesAsync();
        //    }

        //    return Ok("Images uploaded, organized, and saved to DB successfully.");
        //}

        #endregion

        #region Get APIs

        [HttpGet("GetProductDetailsList")]
        public async Task<IActionResult> GetProductDetailsList()
        {
            var result = await _productRepository.GetProductStyleList();
            return Ok(result);
        }

        [HttpGet("GetProductCollectionNewList")]
        public async Task<IActionResult> GetProductNewCollection()
        {
            var result = await _productRepository.GetProductCollectionNewList();
            return Ok(result);
        }

        #endregion


        [HttpPost("upload-merged-json")]
        public async Task<IActionResult> UploadExcelWithMergedCellsToJson(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid file format. Please upload an Excel (.xlsx) file.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(stream);

            foreach (var wkSheet in package.Workbook.Worksheets)
            {
                if (wkSheet.Name == "Engagement Rings")
                {
                    //var ringProducts = ProcessRingData(wkSheet);
                    //await _productRepository.SaveNewProductList(ringProducts);
                }
                else if (wkSheet.Name == "Wedding Bands")
                {
                    //var weddings = ProcessWeddingData(worksheet);
                    //await _weddingRepository.SaveWeddingList(weddings); // Hypothetical method
                }
                else if (wkSheet.Name == "Earrings")
                {
                    //var weddings = ProcessWeddingData(worksheet);
                    //await _weddingRepository.SaveWeddingList(weddings); // Hypothetical method
                }
                else if (wkSheet.Name == "Pendants")
                {
                    //var weddings = ProcessWeddingData(worksheet);
                    //await _weddingRepository.SaveWeddingList(weddings); // Hypothetical method
                }
                else if (wkSheet.Name == "Bracelets")
                {
                    //var weddings = ProcessWeddingData(worksheet);
                    //await _weddingRepository.SaveWeddingList(weddings); // Hypothetical method
                }
            }


            return Ok();
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
                    CategoryName = "Rings",
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
                    product.CategoryName = "Rings";
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
                    CategoryName = "Bands",
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
                    product.CategoryName = "Bands";
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
                    CategoryName = "Earrings",
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
                    product.CategoryName = "Earrings";
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
                    CategoryName = "Pendants",
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
                    product.CategoryName = "Pendants";
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

            for (int row = 5; row <= rowCount; row++)
            {
                product = new ProductDTO
                {
                    CategoryName = "Bracelets",
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
                    product.CategoryName = "Bracelets";
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




        private DateTime? ParseDate(object cellValue)
        {
            if (cellValue is DateTime parsedDate)
            {
                return parsedDate;
            }

            // Fallback for parsing in dd-MM-yyyy format
            if (cellValue is string dateString)
            {
                DateTime result;
                if (DateTime.TryParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    return result;
                }
            }

            // Return null if the value cannot be parsed
            return null;
        }


        //private List<ProductDTO> ProcessRingData(ExcelWorksheet worksheet)
        //{
        //    var products = new List<ProductDTO>();
        //    int rowCount = worksheet.Dimension.Rows;
        //    //Temporary value holders
        //    string tmpLength = "", tmpWidth = "", tmpWeight = "", tmpCenterCarat = "", tmpMMSize = "", tmpColors = "",
        //           tmpVender = "", tmpStyle = "", tmpSku = "", tmpShape = "", tmpGrades = "", tmpCTW = "";
        //    int tmpNoOfStones = 0;
        //    decimal tmpDiaWt = 0, tmpPrice = 0;
        //    int index = 0;
        //    ProductDTO product = null;

        //    for (int row = 5; row <= rowCount; row++)
        //    {
        //        if (index == 0 || index > 3)
        //        {
        //            product = CreateBaseProductDTO(worksheet, row);
        //            product.Id = Guid.NewGuid();
        //            product.CategoryName = "Rings";
        //            product.UnitPrice = product.Price;
        //            index = 1;
        //        }
        //        else
        //        {
        //            product = CreateBaseProductDTO(worksheet, row);
        //            product.Id = Guid.NewGuid();
        //            product.CategoryName = "Rings";
        //            product.UnitPrice = product.Price;

        //            product.Sku = string.IsNullOrWhiteSpace(product.Sku) ? tmpSku : product.Sku;
        //            product.Grades = string.IsNullOrWhiteSpace(product.Grades) ? tmpGrades : product.Grades;
        //            product.StyleName = string.IsNullOrWhiteSpace(product.StyleName) ? tmpStyle : product.StyleName;
        //            product.CenterShapeName = string.IsNullOrWhiteSpace(product.CenterShapeName) ? tmpShape : product.CenterShapeName;
        //            product.VenderName = string.IsNullOrWhiteSpace(product.VenderName) ? tmpVender : product.VenderName;
        //            product.ColorName = string.IsNullOrWhiteSpace(product.ColorName) ? tmpColors : product.ColorName;
        //            product.Length = string.IsNullOrWhiteSpace(product.Length) ? tmpLength : product.Length;
        //            product.BandWidth = string.IsNullOrWhiteSpace(product.BandWidth) ? tmpWidth : product.BandWidth;
        //            product.GoldWeight = string.IsNullOrWhiteSpace(product.GoldWeight) ? tmpWeight : product.GoldWeight;
        //            product.CTW = string.IsNullOrWhiteSpace(product.CTW) ? tmpCTW : product.CTW;
        //            product.CenterCaratName = string.IsNullOrWhiteSpace(product.CenterCaratName) ? tmpCenterCarat : product.CenterCaratName;
        //            product.MMSize = string.IsNullOrWhiteSpace(product.MMSize) ? tmpMMSize : product.MMSize;
        //            product.DiaWT = product.DiaWT == 0 ? tmpDiaWt : product.DiaWT;
        //            product.NoOfStones = product.NoOfStones == 0 ? tmpNoOfStones : product.NoOfStones;
        //            product.Price = product.Price == 0 ? tmpPrice : product.Price;
        //        }

        //        //Update temp values
        //        tmpSku = product.Sku;
        //        tmpGrades = product.Grades;
        //        tmpStyle = product.StyleName;
        //        tmpShape = product.CenterShapeName;
        //        tmpVender = product.VenderName;
        //        tmpColors = product.ColorName;
        //        tmpLength = product.Length;
        //        tmpWidth = product.BandWidth;
        //        tmpWeight = product.GoldWeight;
        //        tmpCTW = product.CTW;
        //        tmpCenterCarat = product.CenterCaratName;
        //        tmpMMSize = product.MMSize;
        //        tmpDiaWt = product.DiaWT ?? 0;
        //        tmpNoOfStones = product.NoOfStones;
        //        tmpPrice = product.Price;

        //        //Split ColorName and create variations
        //        var ringVariants = CreateRingVariantsFromColor(product);
        //        products.AddRange(ringVariants);

        //        index++;
        //    }

        //    return products;
        //}

        private ProductDTO CreateBaseProductDTO(ExcelWorksheet sheet, int row)
        {
            return new ProductDTO
            {
                VenderName = sheet.Cells[row, 6].Text,
                StyleName = sheet.Cells[row, 7].Text,
                Sku = sheet.Cells[row, 7].Text,
                Length = sheet.Cells[row, 9].Text,
                BandWidth = sheet.Cells[row, 10].Text,
                GoldWeight = sheet.Cells[row, 11].Text,
                CTW = sheet.Cells[row, 12].Text,
                CenterShapeName = sheet.Cells[row, 13].Text,
                CenterCaratName = sheet.Cells[row, 14].Text,
                ColorName = sheet.Cells[row, 16].Text,
                Grades = sheet.Cells[row, 21].Text,
                MMSize = sheet.Cells[row, 18].Text,
                DiaWT = decimal.TryParse(sheet.Cells[row, 19].Text, out var diaWt) ? diaWt : 0,
                NoOfStones = int.TryParse(sheet.Cells[row, 20].Text, out var noOfStones) ? noOfStones : 0,
                Price = decimal.TryParse(sheet.Cells[row, 25].Text, out var price) ? price : 0,
                AccentStoneShapeName = sheet.Cells[row, 18].Text,
                Description = sheet.Cells[row, 33].Text,
                WholesaleCost = decimal.TryParse(sheet.Cells[row, 27].Text, out var hCost) ? hCost : 0,
                EventName = sheet.Cells[row, 5].Text,
                ProductType = sheet.Cells[row, 23].Text,
                Certificate = sheet.Cells[row, 15].Text,
                IsReadyforShip = string.IsNullOrWhiteSpace(sheet.Cells[row, 1].Text) && sheet.Cells[row, 1].Text == SD.ReadyToPdp
            };
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
                    WholesaleCost=baseProduct.WholesaleCost,
                    Diameter = baseProduct.Diameter
                });
            }
            return result;
        }
    }
}
