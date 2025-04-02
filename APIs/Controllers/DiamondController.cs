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

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondController : ControllerBase
    {
        private readonly IDiamondRepository _diamondRepository;
        public DiamondController(IDiamondRepository diamondRepository)
        {
            _diamondRepository = diamondRepository;
        }

        [HttpPost("GetDiamondList")]
        public async Task<ActionResult> GetDiamondByFilterData(DiamondFilters diamondFilters)
        {
            var response = _diamondRepository.GetDiamondsAsync(diamondFilters,1,10);
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
                List<ProductDTO> products = new();
                List<DiamondData> diamondList = new();
                var diamondData = new DiamondData();

                for (int row = 2; row <= 96; row++)
                {
                    diamondData = new DiamondData()
                    {
                        LotNo = worksheet.Cells[row, 2].Text,
                        //SrNo= worksheet.Cells[row, 3].Text,
                        LabName = worksheet.Cells[row, 4].Text,
                        LotType = worksheet.Cells[row, 5].Text,
                        CertificateNo = worksheet.Cells[row, 6].Text,
                        // Inscription = worksheet.Cells[row, 7].Text,
                        //ReportDate = Convert.ToDateTime(worksheet.Cells[row, 8].Text),
                        //LabDate = Convert.ToDateTime(worksheet.Cells[row, 9].Text),
                        //OrderNo = worksheet.Cells[row, 10].Text,
                        // Remark = worksheet.Cells[row, 11].Text,
                        ShapeName = worksheet.Cells[row, 12].Text,
                        CaratSizeName = worksheet.Cells[row, 13].Text,
                        ColorName = worksheet.Cells[row, 14].Text,
                        ClarityName = worksheet.Cells[row, 15].Text,
                        MDisc = worksheet.Cells[row, 16].Text,
                        MRate = worksheet.Cells[row, 17].Text,
                        MAmt = worksheet.Cells[row, 18].Text,
                        CutName = worksheet.Cells[row, 19].Text,
                        PolishName = worksheet.Cells[row, 20].Text,
                        SymmetryName = worksheet.Cells[row, 21].Text,
                        FluorName = worksheet.Cells[row, 22].Text,
                        Shade = worksheet.Cells[row, 23].Text,
                        HA = worksheet.Cells[row, 24].Text,
                        EyeClean = worksheet.Cells[row, 25].Text,
                        Luster = worksheet.Cells[row, 26].Text,
                        Milky = worksheet.Cells[row, 27].Text,
                        TableName = worksheet.Cells[row, 28].Text,
                        DepthName = worksheet.Cells[row, 29].Text,
                        RatioName = worksheet.Cells[row, 30].Text,
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
                        InwDate = string.IsNullOrEmpty(worksheet.Cells[row, 65].Text) == true ? null : Convert.ToDateTime(worksheet.Cells[row, 65].Text)
                    };
                    diamondList.Add(diamondData);
                }

                //await _productRepository.SaveProductList(products);
                return Ok(diamondList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
