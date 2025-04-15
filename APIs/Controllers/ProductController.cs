using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Business.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Models;
using System.IO.Compression;
using System.Linq;
using System.Globalization;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
        [RequestSizeLimit(1073741824)]
        public async Task<IActionResult> UploadProductCollectionImages(IFormFile zipFile)
        {
            FileSplitDTO styleName;
            string destinationPath = string.Empty;
            string folderPath = string.Empty;

            if (zipFile == null || zipFile.Length == 0)
                return BadRequest("No file uploaded.");

            var extractedFolder = Path.Combine("UploadedFiles", "Collections");
            Directory.CreateDirectory(extractedFolder);

            var zipPath = Path.Combine(extractedFolder, zipFile.FileName);
            using (var fileStream = new FileStream(zipPath, FileMode.Create))
            {
                await zipFile.CopyToAsync(fileStream);
            }

            var productImgVidos = new List<ProductImages>();
            var prdDT = new ProductImages();
            var fileUpload = new FileManager();

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        entry.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        entry.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                        entry.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                    {
                        // Extract StyleName from image name
                        styleName = _productRepository.ExtractStyleName(entry.Name);

                        // Create folder for StyleName
                        folderPath = Path.Combine(extractedFolder, styleName.DesignNo);
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        // Save extracted image
                        destinationPath = Path.Combine(folderPath, entry.Name);
                        entry.ExtractToFile(destinationPath, overwrite: true);

                        // Save Products
                        int pId = await _productRepository.SaveImageVideoPath(destinationPath);

                        prdDT = new ProductImages();
                        if (entry.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase))
                        {
                            prdDT.VideoId = pId;
                        }
                        else if (styleName.Index == 1)
                        {
                            prdDT.ImageLgId = pId;
                            prdDT.IsDefault = true;
                        }
                        else
                        {
                            prdDT.ImageLgId = pId;
                        }
                        prdDT.ImageIndexNumber = styleName.Index;


                        var prdDesignDT = await _productRepository.GetProductByDesignNo(styleName.DesignNo);
                        if (prdDesignDT != null)
                        {
                            prdDT.ProductId = prdDesignDT.Id.ToString();

                            await _productRepository.SaveImageVideoAsync(prdDT);
                        }

                    }
                }
            }


            return Ok("File Uploaded Successfully.");
        }


        [HttpPost("BulkNewProductUpload")]
        public async Task<IActionResult> UploadNewExcel(IFormFile file)
        {
            var product = new ProductDTO();
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
                string dateString = string.Empty;
                //var ProductDate = new DateTime();
                int index = 0;
                for (int row = 5; row <= rowCount; row++)
                {
                    //dateString = worksheet.Cells[row, 1].Text;
                    //if (!string.IsNullOrEmpty(dateString))
                    //{
                    //    ProductDate = DateTime.ParseExact(dateString, "M/d/yy", CultureInfo.InvariantCulture);
                    //}
                    if (index==0)
                    {
                        index += 1;

                    }

                    if (index==1)
                    {
                        index += 1;
                        product = new ProductDTO
                        {
                            Id = Guid.NewGuid(),
                            //ProductDate = ProductDate,
                            // CategoryName = worksheet.Name,
                            CategoryName = "Rings",
                            VenderName = worksheet.Cells[row, 6].Text,
                            StyleName = worksheet.Cells[row, 7].Text,
                            Sku = worksheet.Cells[row, 7].Text,
                            Length = worksheet.Cells[row, 9].Text,
                            BandWidth = worksheet.Cells[row, 10].Text,
                            CTW = worksheet.Cells[row, 12].Text,
                            ShapeName = worksheet.Cells[row, 13].Text,
                            CenterCaratName = worksheet.Cells[row, 14].Text,
                            Grades = worksheet.Cells[row, 21].Text,
                            ColorName = worksheet.Cells[row, 15].Text,
                        };

                        // Convert numeric values safely
                        product.DiaWT = decimal.TryParse(worksheet.Cells[row, 19].Text, out var diaWt) ? diaWt : 0;
                        product.NoOfStones = int.TryParse(worksheet.Cells[row, 20].Text, out var noOfStones) ? noOfStones : 0;
                        product.Price = decimal.TryParse(worksheet.Cells[row, 25].Text, out var Sprice) ? Sprice : 0;
                        product.UnitPrice = product.Price;
                    }
                    else
                    {
                        if (index > 3)
                        {
                            index = 0;
                        }
                        index += 1;
                        product.BandWidth = worksheet.Cells[row, 10].Text;
                        product.CenterCaratName = worksheet.Cells[row, 14].Text;

                        

                    }


                    // Handling multiple metal colors
                    int lastSpaceIndex = product.ColorName.LastIndexOf(' ');
                    if (lastSpaceIndex == -1)
                    {
                        continue;
                    }
                    string leftPart = product.ColorName.Substring(0, lastSpaceIndex);     // "14k White,Yellow,Rose"
                    string productType = product.ColorName.Substring(lastSpaceIndex + 1); // "Gold"

                    // Now get karat and colors
                    string[] leftParts = leftPart.Split(' ', 2); // Split only once
                    string karat = leftParts[0];                 // "14k"
                    var productMetals = leftParts[1].Split(',');   // ["White", "Yellow", "Rose"]

                    if (productMetals.Length > 0)
                    {
                        //string carat = productMetals[0]; // Store carat separately
                        foreach (var metal in productMetals) // Skip first item (carat)
                        {
                            products.Add(new ProductDTO
                            {
                                Id = product.Id,
                                //CategoryName = product.CategoryName,
                                CategoryName = "Rings",
                                VenderName = product.VenderName,
                                StyleName = product.StyleName,
                                Sku = product.Sku,
                                Length = product.Length,
                                BandWidth = product.BandWidth,
                                GoldWeight = product.GoldWeight,
                                CTW = product.CTW,
                                CenterShapeName = product.CenterShapeName,
                                CenterCaratName = product.CenterCaratName,
                                CaratSizeName = product.CaratSizeName,
                                ShapeName = product.ShapeName,
                                Grades = product.Grades,
                                DiaWT = product.DiaWT,
                                NoOfStones = product.NoOfStones,
                                Price = product.Price,
                                UnitPrice = product.UnitPrice,
                                //CaratName = carat,
                                ColorName = metal.Trim(),
                                Karat = karat,
                                ProductType = productType,
                                IsActivated = true
                            });
                        }
                    }
                }

                await _productRepository.SaveNewProductList(products);
                return Ok("File uploaded and processed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("GetProductsByFilters")]
        public async Task<IActionResult> GetProductsByFilters(ProductFilters filters, int pageNumber = 1, int pageSize = 10)
        {
            var products = await _productRepository.GetProductStyleList();

            var query = products.AsQueryable();

            ////Apply filters
            //if (filters.Carats?.Any() == true)
            //{
            //    query = query.Where(p => filters.Carats.Contains(p.Carat));
            //}
            if (filters.Shapes?.Any() == true)
            {
                query = query.Where(p => p.Shapes.Any(shape => filters.Shapes.Contains(shape.Id.ToString())));
            }
            if (filters.Metals?.Any() == true)
            {
                query = query.Where(p => p.Metals.Any(metal => filters.Metals.Contains(metal.Id.ToString())));
            }
            //if (filters.category.Any() == true)
            //{
            //    query = query.Where(p => filters.category.Contains(p.CategoryName));
            //}
            ////if (filters.subCategory?.Any() == true)
            ////{
            ////    query = query.Where(p => filters.subCategory.Contains(p.SubCategory));
            ////}
            //if (filters.FromPrice.HasValue)
            //{
            //    query = query.Where(p => p.Price >= filters.FromPrice.Value);
            //}
            //if (filters.ToPrice.HasValue)
            //{
            //    query = query.Where(p => p.Price <= filters.ToPrice.Value);
            //}

           // products = await query.ToListAsync();

            return Ok(query);

        }

        [HttpGet("GetProductDetails/{productId}")]
        public async Task<IActionResult> GetProductsByFilters(string productId)
        {
            var products = await _productRepository.GetProductWithDetails(productId);
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
                        CTW = worksheet.Cells[row, 8].Text,
                        CenterShapeName = worksheet.Cells[row, 9].Text,
                        CenterCaratName = worksheet.Cells[row, 10].Text,
                        ColorName = worksheet.Cells[row, 11].Text,
                        ShapeName = worksheet.Cells[row, 12].Text,
                        CaratSizeName = worksheet.Cells[row, 13].Text,
                        Grades = worksheet.Cells[row, 16].Text,
                        WebsiteImagesLink = worksheet.Cells[row, 18].Text
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

        [HttpPost("GetProductDetailsList")]
        public async Task<IActionResult> GetProductDetailsList(ProductFilters filters, int pageNumber = 1, int pageSize = 5000)
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


    }
}
