using Business.Repository.IRepository;
using Common;
using CsvHelper;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Graph.Models;
using Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ControlPanel.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [Authorize]
    public class DiamondController : Controller
    {
        private readonly IDiamondRepository _diamondRepository;
        private readonly IDiamondPropertyRepository _diamondPPTY;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiamondController(IDiamondRepository diamondRepository,
            IDiamondPropertyRepository diamondPPTY, UserManager<ApplicationUser> userManager)
        {
            _diamondRepository = diamondRepository;
            _diamondPPTY = diamondPPTY;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DiamondList(int page = 1, int pageSize = 10)
        {
            var data = await _diamondRepository.GetDiamondList();

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
                return Json(new { status="fail",message = "No file uploaded." });
            }

            var userId = HttpContext.Session.GetString("UserId");
            var user = await _userManager.FindByIdAsync(userId);

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".csv")
            {
                return Json(new { status = "fail", message = "Invalid file format. Only .xlsx or .csv files are supported." });
            }

            try
            {
                // Step 1: Save upload history
                var uploadHistory = new DiamondFileUploadHistoryDTO
                {
                    Title = $"Diamond File Upload with {user.FirstName} {user.LastName}",
                    UploadedDate = DateTime.UtcNow,
                    UploadedBy = userId,
                    IsSuccess = 1
                };
                int uploadHistoryId = await _diamondRepository.AddDiamondFileUploadedHistory(uploadHistory);

                // Step 2: Read file into memory
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Step 3: Parse data
                var diamondsList = extension == ".xlsx"
                    ? await ParseExcelDiamondsAsync(memoryStream, userId)
                    : await ParseCsvDiamondsAsync(memoryStream, userId);

                if (diamondsList == null || diamondsList.Count == 0)
                {
                    return Json(new { status = "fail", message = "No valid diamond records found in the file." });
                }

                // Step 4: Bulk insert
                string jsonData = JsonConvert.SerializeObject(diamondsList);
                var result = await _diamondRepository.BulkInsertDiamondsAsync(jsonData, uploadHistoryId);
                if (result.Count > 0)
                {
                    await _diamondRepository.InsertDiamondHistoryFromDiamondAsync(uploadHistoryId);
                }

                return Json(new { status = "success", message = $"File uploaded successfully. {diamondsList.Count} records inserted." });
            }
            catch (Exception ex)
            {
                return Json(new { status = "fail", message = $"Internal server error: {ex.Message}" });
            }
        }


        private async Task<List<Diamond>> ParseCsvDiamondsAsync(Stream stream, string userId)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<dynamic>().ToList();
            var rowDict = (IDictionary<string, object>)records;
            var diamond = new Diamond();
            var diamonds = new List<Diamond>();

            foreach (var record in records.Skip(5)) // Skip header rows (assumed 1–5)
            {
                diamond = await ParseCSVDiamondRowAsync(rowDict, userId);
                if (diamond != null)
                    diamonds.Add(diamond);
            }

            return diamonds;
        }

        private async Task<List<Diamond>> ParseExcelDiamondsAsync(Stream stream, string userId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null || worksheet.Dimension == null)
                return new List<Diamond>();

            int rowCount = worksheet.Dimension.Rows;
            var diamonds = new List<Diamond>();
            var diamond = new Diamond();
            for (int row = 6; row <= rowCount; row++) // Start from row 6
            {
                diamond = await ParseExcelDiamondRowAsync(worksheet, row, userId);
                if (diamond != null)
                    diamonds.Add(diamond);
            }

            return diamonds;
        }

        [HttpGet]
        public IActionResult GetDiamond(int diamondId)
        {
            ViewBag.Title = "Diamond Details";
            var data = _diamondRepository.GetDiamondById(diamondId);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> DiamondProperty()
        {
            var diamondList = await _diamondPPTY.GetAllAsync();
            return View(diamondList);
        }

        [HttpGet]
        public async Task<IActionResult> DiamondPropertyItems(string propertyName)
        {
            var diamondList = await _diamondPPTY.GetPropertyItemsByName(propertyName);
            return PartialView("~/Views/Diamond/_DiamondPropertyItems.cshtml", diamondList);
        }

        [HttpGet]
        public async Task<IActionResult> AddDiamondProperty(int? pId = 0)
        {
            var data = new DiamondProperty();
            ViewBag.ParentDrp = _diamondPPTY.GetAllAsync().Result.Where(x => x.ParentId == null).ToList();
            ViewBag.Title = "Create Diamond Parameter";
            if (pId.HasValue && pId > 0)
            {
                ViewBag.Title = "Edit Diamond Parameter";
                data = await _diamondPPTY.GetDiamondPropertyByIdAsync(pId.Value);

                return View(data);
            }


            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> AddDiamondProperty(DiamondProperty diamondData, IFormFile UploadFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentDrp = _diamondPPTY.GetAllAsync().Result.Where(x => x.ParentId == null).ToList();

                return View(diamondData);
            }
            if (UploadFile != null && UploadFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "diamondProp");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(UploadFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadFile.CopyToAsync(fileStream);
                }

                // Save file path or name to the database, e.g.:
                diamondData.IconPath = "/diamondProp/" + uniqueFileName;
            }

            if (diamondData != null && diamondData.Id > 0)
            {
                var existingProperty = await _diamondPPTY.GetDiamondPropertyByIdAsync(diamondData.Id);
                if (existingProperty == null)
                {
                    TempData["Status"] = "Fail";
                    TempData["Message"] = "Diamond Property not found.";
                    return View(diamondData);
                }

                existingProperty.Name = !string.IsNullOrWhiteSpace(diamondData.Name) ? diamondData.Name : existingProperty.Name;
                existingProperty.SymbolName = !string.IsNullOrWhiteSpace(diamondData.SymbolName) ? diamondData.SymbolName : existingProperty.SymbolName;
                existingProperty.IconPath = !string.IsNullOrWhiteSpace(diamondData.IconPath) ? diamondData.IconPath : existingProperty.IconPath;
                existingProperty.Description = !string.IsNullOrWhiteSpace(diamondData.Description) ? diamondData.Description : existingProperty.Description;
                existingProperty.IsActivated = diamondData.IsActivated != true ? false : true;
                existingProperty.ParentId = diamondData.ParentId;
                existingProperty.DispOrder = diamondData.DispOrder;

                await _diamondPPTY.UpdateAsync(existingProperty);
            }
            else
            {
                await _diamondPPTY.AddAsync(diamondData);
            }

            TempData["Status"] = "Success";
            TempData["Message"] = "Diamond property has been saved successfully.";

            return RedirectToAction("DiamondProperty");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDiamondProperty(int? pId = 0)
        {
            if (!pId.HasValue || pId <= 0)
            {
                TempData["Status"] = "Fail";
                TempData["Message"] = "Invalid property ID.";
                return RedirectToAction("DiamondProperty");
            }

            var success = await _diamondPPTY.DeleteAsync(pId.Value);

            if (!success)
            {
                TempData["Status"] = "Fail";
                TempData["Message"] = "Diamond property has been failed to deleted.";
                return RedirectToAction("DiamondProperty");
            }
            TempData["Status"] = "Success";
            TempData["Message"] = "Diamond property has been deleted successfully.";
            return RedirectToAction("DiamondProperty");
        }

        private async Task<Diamond> ParseExcelDiamondRowAsync(ExcelWorksheet worksheet, int row, string userId)
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

            var stoneId = worksheet.Cells[row, 2].Text;

            if (string.IsNullOrWhiteSpace(stoneId) || colorId == 0 || shapeId == 0 || labId == 0 || clarityId == 0 || cutId == 0)
            {
                return new Diamond
                {
                    StoneId = stoneId,
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
                    IsActivated = false,
                    IsSuccess = false,
                    UploadStatus = SD.Cancelled,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = userId,
                    CreatedBy = userId
                };

            }



            var IsExist = await _diamondRepository.GetDiamondByStoneId(stoneId);

            if (IsExist != null)
            {
                IsExist.StoneId = stoneId;
                IsExist.DNA = DNA_NewVal;
                IsExist.Step = worksheet.Cells[row, 4].Text;
                IsExist.TypeId = typeId > 0 ? typeId : null;
                IsExist.LabId = labId > 0 ? labId : null;
                IsExist.ShapeId = shapeId > 0 ? shapeId : null;
                IsExist.Carat = Convert.ToDecimal(worksheet.Cells[row, 8].Text);
                IsExist.ClarityId = clarityId > 0 ? clarityId : null;
                IsExist.ColorId = colorId > 0 ? colorId : null;
                IsExist.CutId = cutId > 0 ? cutId : null;
                IsExist.PolishId = polishId > 0 ? polishId : null;
                IsExist.SymmetryId = symmId > 0 ? symmId : null;
                IsExist.FluorId = fluorId > 0 ? fluorId : null;
                IsExist.RAP = Convert.ToDecimal(worksheet.Cells[row, 15].Text);
                IsExist.Discount = Convert.ToDecimal(worksheet.Cells[row, 16].Text);
                IsExist.Price = Convert.ToDecimal(worksheet.Cells[row, 17].Text);
                IsExist.Amount = Convert.ToDecimal(worksheet.Cells[row, 18].Text);
                IsExist.Measurement = worksheet.Cells[row, 19].Text;
                IsExist.Ratio = Convert.ToDecimal(worksheet.Cells[row, 20].Text);
                IsExist.Depth = Convert.ToDecimal(worksheet.Cells[row, 21].Text);
                IsExist.Table = Convert.ToDecimal(worksheet.Cells[row, 22].Text);
                IsExist.Shade = worksheet.Cells[row, 23].Text;
                IsExist.LabShape = worksheet.Cells[row, 24].Text;
                IsExist.RapAmount = Convert.ToDecimal(worksheet.Cells[row, 25].Text);
                IsExist.DiamondImagePath = "-";
                IsExist.DiamondVideoPath = Video_NewVal;
                IsExist.Certificate = Certi_NewVal;
                IsExist.IsActivated = false;
                IsExist.UpdatedDate = DateTime.Now;
                IsExist.UpdatedBy = userId;
                IsExist.IsSuccess = true;

                await _diamondRepository.UpdateDiamondsStatus(IsExist);

                return new Diamond();
            }

            return new Diamond
            {
                StoneId = stoneId,
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
                IsActivated = false,
                UploadStatus = SD.Pending,
                UpdatedDate = DateTime.Now,
                UpdatedBy = userId,
                CreatedBy = userId,
                IsSuccess = true,
            };
        }

        private async Task<Diamond> ParseCSVDiamondRowAsync(IDictionary<string, object> row, string userId)
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
            var diamondHist = new DiamondHistory();
            var DstoneId = GetVal("StoneId");

            if (DstoneId == null || colorId == 0 || shapeId == 0 || labId == 0 || clarityId == 0 || cutId == 0)
            {
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
                    UploadStatus = SD.Cancelled,
                    IsActivated = false,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = userId,
                    IsSuccess = false
                };

            }

            var IsExist = await _diamondRepository.GetDiamondByStoneId(DstoneId);

            if (IsExist != null)
            {
                IsExist.StoneId = GetVal("StoneId");
                IsExist.DNA = GetVal("DNA");
                IsExist.Step = GetVal("Step");
                IsExist.TypeId = typeId > 0 ? typeId : null;
                IsExist.LabId = labId > 0 ? labId : null;
                IsExist.ShapeId = shapeId > 0 ? shapeId : null;
                IsExist.Carat = Convert.ToDecimal(GetVal("Carat"));
                IsExist.ClarityId = clarityId > 0 ? clarityId : null;
                IsExist.ColorId = colorId > 0 ? colorId : null;
                IsExist.CutId = cutId > 0 ? cutId : null;
                IsExist.PolishId = polishId > 0 ? polishId : null;
                IsExist.SymmetryId = symmId > 0 ? symmId : null;
                IsExist.FluorId = fluorId > 0 ? fluorId : null;
                IsExist.RAP = Convert.ToDecimal(GetVal("RAP"));
                IsExist.Discount = Convert.ToDecimal(GetVal("Discount"));
                IsExist.Price = Convert.ToDecimal(GetVal("Price"));
                IsExist.Amount = Convert.ToDecimal(GetVal("Amount"));
                IsExist.Measurement = GetVal("Measurement");
                IsExist.Ratio = Convert.ToDecimal(GetVal("Ratio"));
                IsExist.Depth = Convert.ToDecimal(GetVal("Depth"));
                IsExist.Table = Convert.ToDecimal(GetVal("Table"));
                IsExist.Shade = GetVal("Shade");
                IsExist.LabShape = GetVal("LabShape");
                IsExist.RapAmount = Convert.ToDecimal(GetVal("RapAmount"));
                IsExist.DiamondImagePath = "-";
                IsExist.DiamondVideoPath = GetVal("Video");
                IsExist.Certificate = GetVal("Certificate");
                IsExist.IsActivated = false;
                IsExist.UpdatedDate = DateTime.Now;
                IsExist.UpdatedBy = userId;
                IsExist.IsSuccess = true;
                await _diamondRepository.UpdateDiamondsStatus(IsExist);

                return new Diamond();
            }

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
                IsActivated = false,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                UploadStatus = SD.Pending,
                UpdatedBy = userId,
                CreatedBy = userId,
                IsSuccess = true
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

        //Raj 
        public IActionResult JqxDemo()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getDiamondList(string uploadStatus, bool isActive)
        {
            bool status = false;
            string strResult = "";
            string strMessage = "Data Not Found";

            var data = await _diamondRepository.GetDiamondListByStatus(uploadStatus, isActive);
            if (data != null)
            {
                status = true;
                strMessage = "";
                strResult = JsonConvert.SerializeObject(data);
            }
            return Json(new
            {
                Data = new
                {
                    status = status,
                    result = strResult,
                    message = strMessage
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> getFailDiamondList(string uploadStatus, bool isActive)
        {
            bool status = false;
            string strResult = "";
            string strMessage = "Data Not Found";

            var data = await _diamondRepository.GetDiamondListByStatus(uploadStatus, isActive);
            if (data != null)
            {
                var failResponse = data.Where(x => x.IsSuccess == false).ToList();
                status = true;
                strMessage = "";
                strResult = JsonConvert.SerializeObject(failResponse);
            }
            return Json(new
            {
                failResponse = new
                {
                    status = status,
                    result = strResult,
                    message = strMessage
                }
            });
        }

        [HttpGet]
        public IActionResult DiamondVerification()
        {
            return View();
        }


        [HttpGet]
        public IActionResult DiamondHold()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeactivatedDiamond()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDiamondVerificationList(int? fileHistoryId = 0)
        {
            bool status = false;
            string strResult = "";
            string strMessage = "Data Not Found";

            var data = await _diamondRepository.GetPendingDiamondList();
            if (fileHistoryId > 0)
            {
            }

            if (data != null)
            {
                status = true;
                strMessage = "";
                strResult = JsonConvert.SerializeObject(data);
            }
            return Json(new
            {
                Data = new
                {
                    status = status,
                    result = strResult,
                    message = strMessage
                }
            });
        }


        //[HttpPost]
        //public async Task<IActionResult> ChangeStatus(string[] stoneIds, string status)
        //{
        //    var userId = HttpContext.Session.GetString("UserId");
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (stoneIds != null && stoneIds.Length > 0)
        //    {
        //        var response = await _diamondRepository.UpdateDiamondsStatus(stoneIds, userId, status);
        //        if (response == true)
        //        {
        //            return Json($"Diamonds status have been change successfully. status : {status}");
        //        }
        //        else
        //        {
        //            return Json($"Diamonds status have been failed for change status. please try again.");
        //        }
        //    }
        //    else
        //    {
        //        return Json($"Please select diamonds.");
        //    }

        //}


        public class StoneStatusChangeModel
        {
            public string[] StoneIds { get; set; }
            public string Status { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus([FromBody] StoneStatusChangeModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var user = await _userManager.FindByIdAsync(userId);

            if (model.StoneIds != null && model.StoneIds.Length > 0)
            {
                var response = await _diamondRepository.UpdateDiamondsStatus(model.StoneIds, userId, model.Status);
                if (response == true)
                {
                    return Json(new { success = true, message = $"Diamonds status have been changed successfully. status : {model.Status}" });
                }
                else
                {
                    return Json(new { success = false, message = "Diamonds status have been failed for change status. please try again." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Please select diamonds." });
            }

        }

        [HttpGet]
        public IActionResult GetFileUploadHistory() { return View(); }

        [HttpGet]
        public async Task<IActionResult> ShowFileUploadHistory()
        {
            bool status = false;
            string strResult = "";
            string strMessage = "Data Not Found";

            var response = await _diamondRepository.GetDiamondFileUploadedHistoryList();
            if (response != null)
            {
                status = true;
                strMessage = "";
                strResult = JsonConvert.SerializeObject(response);
            }
            return Json(new
            {
                Data = new
                {
                    status = status,
                    result = strResult,
                    message = strMessage
                }
            });
        }

        [HttpGet]
        public IActionResult ThankYouForUploaded()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetDiamondHistoryById(string diamondId)
        {
            bool status = false;
            string strResult = "";
            string strMessage = "Data Not Found";

            var data = await _diamondRepository.GetDiamondHistoryById(diamondId);
            if (data != null)
            {
                status = true;
                strMessage = "";
                strResult = JsonConvert.SerializeObject(data);
            }
            return Json(new
            {
                Data = new
                {
                    status = status,
                    result = strResult,
                    message = strMessage
                }
            });
        }
    }
}
