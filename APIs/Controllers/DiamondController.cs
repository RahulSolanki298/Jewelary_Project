using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using OfficeOpenXml;
using DataAccess.Entities;
using Business.Repository.IRepository;
using Common;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using CsvHelper;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondController : ControllerBase
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPropertyRepository _diamondPPTY;
        public DiamondController(IDiamondRepository diamondRepository, IDiamondPropertyRepository diamondPPTY)
        {
            _diamondRepository = diamondRepository;
            _diamondPPTY = diamondPPTY;
        }

        [HttpPost("GetDiamondList")]
        public async Task<ActionResult> GetDiamondByFilterData(DiamondFilters diamondFilters, int pageNumber = 1, int pageSize = 10)
        {
            var response = await _diamondRepository.GetDiamondsAsync(diamondFilters, pageNumber, pageSize);
            return Ok(response);
        }

        [HttpPost("GetAllDiamond")]
        public async Task<ActionResult> GetDiamondList()
        {
            var response = await _diamondRepository.GetDiamondList();
            return Ok(response);
        }

        [HttpPost("getDiamondListBydiamondIds")]
        public async Task<ActionResult> GetDiamondListByIds(int[] diamondIds)
        {
            if (diamondIds.Length == 0)
            {
                return BadRequest("No diamond IDs provided.");
            }

            try
            {

                var response = await _diamondRepository.GetDiamondList();

                var filteredDiamonds = response.Where(x => diamondIds.Contains(x.Id)).ToList();

                if (!filteredDiamonds.Any())
                {
                    return NotFound("No diamonds found with the provided IDs.");
                }

                return Ok(filteredDiamonds);
            }
            catch (FormatException ex)
            {
                return BadRequest($"Invalid diamond ID format: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        


        [HttpGet("GetDiamond/diamondId/{diamondId}")]
        public ActionResult GetDiamondById(int diamondId)
        {
            var response = _diamondRepository.GetDiamondById(diamondId);
            return Ok(response);
        }

        //[HttpPost("BulkDiamondUpload")]
        //public async Task<IActionResult> UploadDiamondWithExcelOrCsv(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    var extension = Path.GetExtension(file.FileName);
        //    if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) &&
        //        !extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return BadRequest("Invalid file format. Please upload an Excel (.xlsx) or CSV (.csv) file.");
        //    }

        //    try
        //    {
        //        List<Diamond> diamondsDTList = new();

        //        using var stream = new MemoryStream();
        //        await file.CopyToAsync(stream);
        //        stream.Position = 0;
        //        var history = new DiamondFileUploadHistory();
        //        history.Title = "Add File Upload";
        //        history.UploadedDate = DateTime.Now;
        //        history.IsSuccess = 1;
        //        int dId = await _diamondRepository.AddDiamondFileUploadedHistory(history);

        //        if (extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        //        {
        //            // Excel Processing
        //            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //            using var package = new ExcelPackage(stream);
        //            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        //            if (worksheet == null)
        //                return BadRequest("The Excel file is empty.");

        //            int rowCount = worksheet.Dimension.Rows;

        //            for (int row = 6; row <= rowCount; row++)
        //            {
        //                var diamond = await ParseExcelDiamondRowAsync(worksheet, row);
        //                diamondsDTList.Add(diamond);
        //            }
        //        }
        //        else if (extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
        //        {
        //            // CSV Processing
        //            using var reader = new StreamReader(stream);
        //            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        //            var records = csv.GetRecords<dynamic>().ToList();
        //            int rowIndex = 6; // Assuming headers end before row 6

        //            foreach (var record in records.Skip(rowIndex - 1))
        //            {
        //                var rowDict = (IDictionary<string, object>)record;
        //                var diamond = await ParseCSVDiamondRowAsync(rowDict);
        //                diamondsDTList.Add(diamond);
        //            }
        //        }

        //        string jsonData = JsonConvert.SerializeObject(diamondsDTList);
        //        var result = await _diamondRepository.BulkInsertDiamondsAsync(jsonData, dId);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

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
