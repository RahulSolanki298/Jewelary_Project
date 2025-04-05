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

        [HttpPost("GetAllDiamond")]
        public async Task<ActionResult> GetDiamondList()
        {
            var response = await _diamondRepository.GetDiamondList();
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
                int labId,typeId, colorId, shapeId, caratId, clarityId, cutId, polishId, symmId, fluorId, tableId, depthId, ratioId;

                for (int row = 6; row <= rowCount; row++)
                {

                    typeId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 5].Text, SD._TYPE);
                    labId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row,6].Text, SD.Lab);
                    colorId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 9].Text, SD.Color);
                    shapeId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 7].Text, SD.Shape);
                    caratId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 8].Text, SD.Carat);
                    clarityId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 10].Text, SD.Clarity);
                    cutId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 11].Text, SD.Cut);
                    polishId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 12].Text, SD.Polish);
                    symmId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 13].Text, SD.Symmetry);
                    fluorId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 14].Text, SD.Fluor);
                    tableId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 28].Text, SD.Table);
                    depthId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 29].Text, SD.Depth);
                    ratioId = await _diamondPPTY.GetDiamondPropertyId(worksheet.Cells[row, 20].Text, SD.Ratio);

                    var DNA_Cell = worksheet.Cells[row, 3];
                    var DNA_NewVal = DNA_Cell.Hyperlink?.AbsoluteUri;

                    var Video_Cell = worksheet.Cells[row, 26];
                    var Video_NewVal = DNA_Cell.Hyperlink?.AbsoluteUri;

                    var Certi_Cell = worksheet.Cells[row, 27];
                    var Certi_NewVal = DNA_Cell.Hyperlink?.AbsoluteUri;

                    diamond = new Diamond()
                    {
                        StoneId= worksheet.Cells[row, 2].Text,
                        DNA= DNA_NewVal,
                        Step= worksheet.Cells[row, 4].Text,
                        TypeId= typeId > 0 ? typeId : null,
                        LabId = labId > 0 ? labId : null,
                        ShapeId = shapeId > 0 ? shapeId : null,
                        CaratId= caratId > 0 ? caratId : null,
                        ClarityId = clarityId > 0 ? clarityId : null,
                        ColorId = colorId > 0 ? colorId : null,
                        CutId = cutId > 0 ? cutId : null,
                        PolishId = polishId > 0 ? polishId : null,
                        SymmetryId = symmId > 0 ? symmId : null,
                        FluorId = fluorId > 0 ? fluorId : null,
                        RAP= Convert.ToDecimal(worksheet.Cells[row, 15].Text),
                        Discount= Convert.ToDecimal(worksheet.Cells[row, 16].Text),
                        Price= Convert.ToDecimal(worksheet.Cells[row, 17].Text),
                        Amount= Convert.ToDecimal(worksheet.Cells[row, 18].Text),
                        Measurement= worksheet.Cells[row, 19].Text,
                        RatioId = ratioId > 0 ? ratioId : null,
                        Depth = Convert.ToDecimal(worksheet.Cells[row, 21].Text),
                        Table = Convert.ToDecimal(worksheet.Cells[row, 22].Text),
                        Shade = worksheet.Cells[row, 23].Text,
                        LabShape = worksheet.Cells[row, 24].Text,
                        RapAmount= Convert.ToDecimal(worksheet.Cells[row, 25].Text),
                        DiamondVideoPath = Video_NewVal,
                        Certificate = Certi_NewVal,
                        IsActivated=true
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

        [HttpGet("GetDiamond/diamondId/{diamondId}")]
        public ActionResult GetDiamondById(int diamondId)
        {
            var response = _diamondRepository.GetDiamondById(diamondId);
            return Ok(response);
        }
    }
}
