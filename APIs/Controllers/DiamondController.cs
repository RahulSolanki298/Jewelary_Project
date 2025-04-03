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

        [HttpPost("BulkDiamondUpload")]
        public async Task<IActionResult> UploadDiamondWithExcel(IFormFile file)
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
                List<Diamond> diamondsDTList = new();
                Diamond diamond = new();
                int labId, colorId, shapeId, caratSizeId, clarityId, cutId, polishId, symmId, fluorId, tableId, depthId, ratioId;

                for (int row = 2; row <= 95; row++)
                {

                    labId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 4].Text, SD.Lab);
                    colorId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 14].Text, SD.Color);
                    shapeId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 12].Text, SD.Shape);
                    caratSizeId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 13].Text, SD.CaratSize);
                    clarityId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 15].Text, SD.Clarity);
                    cutId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 19].Text, SD.Cut);
                    polishId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 20].Text, SD.Polish);
                    symmId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 21].Text, SD.Symmetry);
                    fluorId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 22].Text, SD.Fluor);
                    tableId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 28].Text, SD.Table);
                    depthId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 29].Text, SD.Depth);
                    ratioId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 30].Text, SD.Ratio);

                    diamond = new Diamond()
                    {
                        LotNo = worksheet.Cells[row, 2].Text,
                        //SrNo= worksheet.Cells[row, 3].Text,
                        LabId = labId > 0 ? labId : null,
                        LotType = worksheet.Cells[row, 5].Text,
                        CertificateNo = worksheet.Cells[row, 6].Text,
                        // Inscription = worksheet.Cells[row, 7].Text,
                        //ReportDate = Convert.ToDateTime(worksheet.Cells[row, 8].Text),
                        //LabDate = Convert.ToDateTime(worksheet.Cells[row, 9].Text),
                        //OrderNo = worksheet.Cells[row, 10].Text,
                        // Remark = worksheet.Cells[row, 11].Text,
                        ShapeId = shapeId > 0 ? shapeId : null,
                        CaratSizeId = caratSizeId > 0 ? caratSizeId : null,
                        ColorId = colorId > 0 ? colorId : null,
                        ClarityId = clarityId > 0 ? clarityId : null,
                        MDisc = worksheet.Cells[row, 16].Text,
                        MRate = worksheet.Cells[row, 17].Text,
                        MAmt = worksheet.Cells[row, 18].Text,
                        CutId = cutId > 0 ? cutId : null,
                        PolishId = polishId > 0 ? polishId : null,
                        SymmetryId = symmId > 0 ? symmId : null,
                        FluorId = fluorId > 0 ? fluorId : null,
                        Shade = worksheet.Cells[row, 23].Text,
                        HA = worksheet.Cells[row, 24].Text,
                        EyeClean = worksheet.Cells[row, 25].Text,
                        Luster = worksheet.Cells[row, 26].Text,
                        Milky = worksheet.Cells[row, 27].Text,
                        TableId = tableId > 0 ? tableId : null,
                        DepthId = depthId > 0 ? depthId : null,
                        RatioId = ratioId > 0 ? ratioId : null,
                        Diam = worksheet.Cells[row, 31].Text,
                        Length = Convert.ToDecimal(worksheet.Cells[row, 32].Text),
                        Width = Convert.ToDecimal(worksheet.Cells[row, 33].Text),
                        Height = Convert.ToDecimal(worksheet.Cells[row, 34].Text),
                        CAngle = Convert.ToDecimal(worksheet.Cells[row, 35].Text),
                        CHt = Convert.ToDecimal(worksheet.Cells[row, 36].Text),
                        PAngle = Convert.ToDecimal(worksheet.Cells[row, 37].Text),
                        PHt = Convert.ToDecimal(worksheet.Cells[row, 38].Text),
                        Girdle = worksheet.Cells[row, 39].Text,
                        StrLan = worksheet.Cells[row, 40].Text,
                        LrHalf = worksheet.Cells[row, 41].Text,
                        GirdleDesc = worksheet.Cells[row, 42].Text,
                        Culet = worksheet.Cells[row, 43].Text,
                        KeyToSymbol = worksheet.Cells[row, 44].Text,
                        LabComment = worksheet.Cells[row, 45].Text,
                        LabShape = worksheet.Cells[row, 46].Text,
                        TableWhite = worksheet.Cells[row, 47].Text,
                        SideWhite = worksheet.Cells[row, 48].Text,
                        TableBlack = worksheet.Cells[row, 49].Text,
                        OpenTable = worksheet.Cells[row, 50].Text,
                        OpenCrown = worksheet.Cells[row, 51].Text,
                        OpenPavallion = worksheet.Cells[row, 52].Text,
                        OpenGirdle = worksheet.Cells[row, 53].Text,
                        NT_INT = worksheet.Cells[row, 54].Text,
                        PavExFac = worksheet.Cells[row, 55].Text,
                        TableSpot = worksheet.Cells[row, 56].Text,
                        SideSpot = worksheet.Cells[row, 57].Text,
                        DiamondImagePath = worksheet.Cells[row, 58].Text,
                        DiamondVideoPath = worksheet.Cells[row, 59].Text,
                        OLD_PID = worksheet.Cells[row, 60].Text,
                        ORAP = worksheet.Cells[row, 61].Text,
                        MfgRemark = worksheet.Cells[row, 62].Text,
                        CrownExFac = worksheet.Cells[row, 63].Text,
                        //INWDate = string.IsNullOrEmpty(worksheet.Cells[row, 65].Text) == true ? null : Convert.ToDateTime(worksheet.Cells[row, 65].Text)

                    };
                    diamondsDTList.Add(diamond);
                }
                string jsonData = JsonConvert.SerializeObject(diamondsDTList);
                var result = await _diamondRepository.BulkInsertDiamondsAsync(jsonData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
