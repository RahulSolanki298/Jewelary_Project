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

        [HttpPost("BulkProductUpload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
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
                                VenderName = worksheet.Cells[row, 1].Text,  // Column A
                                StyleName = worksheet.Cells[row, 2].Text,  // Column A
                                ProductType = worksheet.Cells[row, 3].Text, // Column B
                                CategoryName = worksheet.Cells[row,4].Text, // Column C
                                SubCategoryName = worksheet.Cells[row,5].Text, // Column C
                                GoldPurity= worksheet.Cells[row, 6].Text,
                                GoldWeight = worksheet.Cells[row, 7].Text,
                                CTW = worksheet.Cells[row, 8].Text, // Column C
                                ColorName = worksheet.Cells[row, 9].Text, // Column C
                                ShapeName = worksheet.Cells[row, 10].Text, // Column C
                                CaratName = worksheet.Cells[row, 11].Text,
                                CaratSizeName = worksheet.Cells[row, 12].Text,
                                ClarityName = worksheet.Cells[row, 13].Text,
                                Sku = worksheet.Cells[row, 14].Text,
                                Price = Convert.ToDecimal(worksheet.Cells[row, 15].Text),
                                UnitPrice = Convert.ToDecimal(worksheet.Cells[row, 16].Text),
                                Quantity = Convert.ToInt32(worksheet.Cells[row, 17].Text),
                                Id = Guid.NewGuid()
                            };
                            rows.Add(data);
                        }

                        await _productRepository.SaveProductList(rows);

                    }
                }

                return Ok("File uploaded and processed successfully.");
            }
            catch (System.Exception ex)
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
                        for (int row = 5; row <= worksheet.Dimension.Rows; row++) // Start at row 2 to skip header
                        {
                            var data = new ProductDTO
                            {
                                VenderName = worksheet.Cells[row, 3].Text,  // Column A
                                StyleName = worksheet.Cells[row, 4].Text, // Column B
                                GoldPurity = worksheet.Cells[row, 5].Text, // Column C
                                GoldWeight = worksheet.Cells[row, 6].Text, // Column C
                                CTW = worksheet.Cells[row, 7].Text, // Column C
                                CenterShapeName= worksheet.Cells[row,8].Text,
                                CenterCaratName = worksheet.Cells[row,9].Text,
                                ColorName = worksheet.Cells[row, 10].Text,
                                CaratSizeName = worksheet.Cells[row, 7].Text,
                                ClarityName = worksheet.Cells[row, 8].Text,
                                Sku = worksheet.Cells[row, 9].Text,
                                Price = Convert.ToDecimal(worksheet.Cells[row, 10].Text),
                                UnitPrice = Convert.ToDecimal(worksheet.Cells[row, 11].Text),
                                Quantity = Convert.ToInt32(worksheet.Cells[row, 12].Text),
                                CollectionName = worksheet.Cells[row, 13].Text,
                                IsActivated = Convert.ToBoolean(worksheet.Cells[row, 14].Text),
                                Id = Guid.NewGuid()
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
            string fileName = string.Empty;
            string[] fileNameParts;
            string productType, category, color, shape, indexNumber, ext, style, skuNumber;

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
                        string styleName = _productRepository.ExtractStyleName(entry.Name);

                        // Create folder for StyleName
                        string folderPath = Path.Combine(extractedFolder, styleName);
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

        [HttpPost("BulkProductCollectionImagesUpload")]
        public async Task<IActionResult> UploadProductCollectionImages(IFormFile zipFile)
        {
            string fileName = string.Empty;
            string[] fileNameParts;
            string productType, category, color, shape, indexNumber, ext, style, skuNumber;

            if (zipFile == null || zipFile.Length == 0)
                return BadRequest("No file uploaded.");

            var extractedFolder = Path.Combine("UploadedFiles", "Collections");
            Directory.CreateDirectory(extractedFolder);

            var zipPath = Path.Combine(extractedFolder, zipFile.FileName);
            using (var fileStream = new FileStream(zipPath, FileMode.Create))
            {
                await zipFile.CopyToAsync(fileStream);
            }

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
                        string styleName = _productRepository.ExtractStyleName(entry.Name);

                        // Create folder for StyleName
                        string folderPath = Path.Combine(extractedFolder, styleName);
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

        [HttpPost("GetProductDetailsList")]
        public async Task<IActionResult> GetProductDetailsList()
        {
            var result = await _productRepository.GetProductCollectionList();

            return Ok(result);
        }
    }
}
