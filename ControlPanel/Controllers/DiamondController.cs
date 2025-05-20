using Business.Repository.IRepository;
using CsvHelper;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;
using Common;

namespace ControlPanel.Controllers
{
    public class DiamondController : Controller
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPropertyRepository _diamondPPTY;

        public DiamondController(IDiamondRepository diamondRepository, IDiamondPropertyRepository diamondPPTY)
        {
            _diamondRepository = diamondRepository;
            _diamondPPTY = diamondPPTY;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _diamondRepository.GetDiamondList();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> DiamondList(int page = 1, int pageSize = 10)
        {
            var data= await _diamondRepository.GetDiamondList();

            var totalItems = data.Count();

            var diamonds = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;

            return PartialView("_DiamondList", data);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertDiamonds(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["UploadError"] = "No file uploaded.";
                return RedirectToAction("UploadView"); // change this to your actual view
            }

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".csv")
            {
                TempData["UploadError"] = "Invalid file format. Only .xlsx or .csv files are supported.";
                return RedirectToAction("Index");
            }

            try
            {
                // Step 1: Save upload history
                var uploadHistory = new DiamondFileUploadHistory
                {
                    Title = "Add File Upload",
                    UploadedDate = DateTime.UtcNow,
                    IsSuccess = 1
                };
                int uploadHistoryId = await _diamondRepository.AddDiamondFileUploadedHistory(uploadHistory);

                // Step 2: Read file into memory
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Step 3: Parse data
                var diamondsList = extension == ".xlsx"
                    ? await ParseExcelDiamondsAsync(memoryStream)
                    : await ParseCsvDiamondsAsync(memoryStream);

                if (diamondsList == null || diamondsList.Count == 0)
                {
                    TempData["UploadError"] = "No valid diamond records found in the file.";
                    return RedirectToAction("Index");
                }

                // Step 4: Bulk insert
                string jsonData = JsonConvert.SerializeObject(diamondsList);
                var result = await _diamondRepository.BulkInsertDiamondsAsync(jsonData, uploadHistoryId);

                TempData["UploadSuccess"] = $"File uploaded successfully. {diamondsList.Count} records inserted.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["UploadError"] = $"Internal server error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private async Task<List<Diamond>> ParseCsvDiamondsAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<dynamic>().ToList();

            var diamonds = new List<Diamond>();

            foreach (var record in records.Skip(5)) // Skip header rows (assumed 1–5)
            {
                var rowDict = (IDictionary<string, object>)record;
                var diamond = await ParseCSVDiamondRowAsync(rowDict);
                if (diamond != null)
                    diamonds.Add(diamond);
            }

            return diamonds;
        }

        private async Task<List<Diamond>> ParseExcelDiamondsAsync(Stream stream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null || worksheet.Dimension == null)
                return new List<Diamond>();

            int rowCount = worksheet.Dimension.Rows;
            var diamonds = new List<Diamond>();

            for (int row = 6; row <= rowCount; row++) // Start from row 6
            {
                var diamond = await ParseExcelDiamondRowAsync(worksheet, row);
                if (diamond != null)
                    diamonds.Add(diamond);
            }

            return diamonds;
        }

        [HttpGet]
        public IActionResult GetDiamond(int diamondId)
        {
            var data = _diamondRepository.GetDiamondById(diamondId);
            return View(data);
        }

        [HttpGet]
        public IActionResult DiamondProperty()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpsertProperty(DiamondData diamondData)
        {
            return View();
        }

        private async Task<Diamond> ParseExcelDiamondRowAsync(ExcelWorksheet worksheet, int row)
        {
            var Video_NewVal = GetExcelHyperlink(worksheet.Cells[row, 26]);
            var DNA_NewVal = GetExcelHyperlink(worksheet.Cells[row, 3]);
            var Certi_NewVal = GetExcelHyperlink(worksheet.Cells[row, 27]);


            int typeId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 5].Text, SD._TYPE);
            int labId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 6].Text, SD.Lab);
            int shapeId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 7].Text, SD.Shape);
            int colorId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 9].Text, SD.Color);
            int clarityId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 10].Text, SD.Clarity);
            int cutId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 11].Text, SD.Cut);
            int polishId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 12].Text, SD.Polish);
            int symmId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 13].Text, SD.Symmetry);
            int fluorId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 14].Text, SD.Fluor);

            return new Diamond
            {
                StoneId = worksheet.Cells[row, 2].Text,
                DNA = DNA_NewVal,
                Step = worksheet.Cells[row, 4].Text,
                TypeId = typeId > 0 ? typeId : null,
                LabId = labId > 0 ? labId : null,
                ShapeId = shapeId > 0 ? shapeId : null,
                Carat = Convert.ToDecimal(worksheet.Cells[row, 8].Text),
                ClarityId = clarityId > 0 ? clarityId : null,
                ColorId = colorId > 0 ? colorId : null,
                CutId = cutId > 0 ? cutId : null,
                PolishId = polishId > 0 ? polishId : null,
                SymmetryId = symmId > 0 ? symmId : null,
                FluorId = fluorId > 0 ? fluorId : null,
                RAP = Convert.ToDecimal(worksheet.Cells[row, 15].Text),
                Discount = Convert.ToDecimal(worksheet.Cells[row, 16].Text),
                Price = Convert.ToDecimal(worksheet.Cells[row, 17].Text),
                Amount = Convert.ToDecimal(worksheet.Cells[row, 18].Text),
                Measurement = worksheet.Cells[row, 19].Text,
                Ratio = Convert.ToDecimal(worksheet.Cells[row, 20].Text),
                Depth = Convert.ToDecimal(worksheet.Cells[row, 21].Text),
                Table = Convert.ToDecimal(worksheet.Cells[row, 22].Text),
                Shade = worksheet.Cells[row, 23].Text,
                LabShape = worksheet.Cells[row, 24].Text,
                RapAmount = Convert.ToDecimal(worksheet.Cells[row, 25].Text),
                DiamondImagePath = "-",
                DiamondVideoPath = Video_NewVal,
                Certificate = Certi_NewVal,
                IsActivated = true
            };
        }


        private async Task<Diamond> ParseCSVDiamondRowAsync(IDictionary<string, object> row)
        {
            string GetVal(string key) => row.ContainsKey(key) ? row[key]?.ToString() : null;

            int typeId = await _diamondPPTY.GetDiamondPropertyId(GetVal("Type"), SD._TYPE);
            int labId = await _diamondPPTY.GetDiamondPropertyId(GetVal("Lab"), SD.Lab);
            int shapeId = await _diamondPPTY.GetDiamondPropertyId(GetVal("Shape"), SD.Shape);
            int colorId = await _diamondPPTY.GetDiamondPropertyId(GetVal("Color"), SD.Color);
            int clarityId = await _diamondPPTY.GetDiamondPropertyId(GetVal("Clarity"), SD.Clarity);
            int cutId = await _diamondPPTY.GetDiamondPropertyId(GetVal("Cut"), SD.Cut);
            int polishId = await _diamondPPTY.GetDiamondPropertyId(GetVal("Polish"), SD.Polish);
            int symmId = await _diamondPPTY.GetDiamondPropertyId(GetVal("Symmetry"), SD.Symmetry);
            int fluorId = await _diamondPPTY.GetDiamondPropertyId(GetVal("Fluorescence"), SD.Fluor);

            return new Diamond
            {
                StoneId = GetVal("StoneId"),
                DNA = GetVal("DNA"),
                Step = GetVal("Step"),
                TypeId = typeId > 0 ? typeId : null,
                LabId = labId > 0 ? labId : null,
                ShapeId = shapeId > 0 ? shapeId : null,
                Carat = Convert.ToDecimal(GetVal("Carat")),
                ClarityId = clarityId > 0 ? clarityId : null,
                ColorId = colorId > 0 ? colorId : null,
                CutId = cutId > 0 ? cutId : null,
                PolishId = polishId > 0 ? polishId : null,
                SymmetryId = symmId > 0 ? symmId : null,
                FluorId = fluorId > 0 ? fluorId : null,
                RAP = Convert.ToDecimal(GetVal("RAP")),
                Discount = Convert.ToDecimal(GetVal("Discount")),
                Price = Convert.ToDecimal(GetVal("Price")),
                Amount = Convert.ToDecimal(GetVal("Amount")),
                Measurement = GetVal("Measurement"),
                Ratio = Convert.ToDecimal(GetVal("Ratio")),
                Depth = Convert.ToDecimal(GetVal("Depth")),
                Table = Convert.ToDecimal(GetVal("Table")),
                Shade = GetVal("Shade"),
                LabShape = GetVal("LabShape"),
                RapAmount = Convert.ToDecimal(GetVal("RapAmount")),
                DiamondImagePath = "-",
                DiamondVideoPath = GetVal("Video"),
                Certificate = GetVal("Certificate"),
                IsActivated = true
            };
        }


        public static string GetExcelHyperlink(ExcelRange cell)
        {
            // 1. Try getting the hyperlink object
            if (cell.Hyperlink != null)
            {
                return cell.Hyperlink.AbsoluteUri;
            }

            // 2. If no hyperlink object, try to parse formula-based hyperlink
            if (!string.IsNullOrEmpty(cell.Formula) &&
                cell.Formula.StartsWith("HYPERLINK", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    // Extract URL from formula like =HYPERLINK("url", "text")
                    var formula = cell.Formula;
                    int firstQuote = formula.IndexOf('"');
                    int secondQuote = formula.IndexOf('"', firstQuote + 1);
                    if (firstQuote >= 0 && secondQuote > firstQuote)
                    {
                        return formula.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
                    }
                }
                catch
                {
                    // Log or handle parse error if needed
                    return null;
                }
            }

            return null; // No hyperlink found
        }

    }
}
