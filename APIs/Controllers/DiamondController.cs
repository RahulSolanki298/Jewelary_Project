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


        [HttpPost("BulkDiamondUpload")]
        public async Task<IActionResult> UploadDiamondWithExcelOrCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var extension = Path.GetExtension(file.FileName);
            if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) &&
                !extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid file format. Please upload an Excel (.xlsx) or CSV (.csv) file.");
            }

            try
            {
                List<Diamond> diamondsDTList = new();

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                if (extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    // Excel Processing
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using var package = new ExcelPackage(stream);
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (worksheet == null)
                        return BadRequest("The Excel file is empty.");

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 6; row <= rowCount; row++)
                    {
                        var diamond = await ParseExcelDiamondRowAsync(worksheet, row);
                        diamondsDTList.Add(diamond);
                    }
                }
                else if (extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    // CSV Processing
                    using var reader = new StreamReader(stream);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                    var records = csv.GetRecords<dynamic>().ToList();
                    int rowIndex = 6; // Assuming headers end before row 6

                    foreach (var record in records.Skip(rowIndex - 1))
                    {
                        var rowDict = (IDictionary<string, object>)record;
                        var diamond = await ParseCSVDiamondRowAsync(rowDict);
                        diamondsDTList.Add(diamond);
                    }
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

        ////public String SaveMigrateImportPurchaseData(DataTable tblData, string strPurType, string strPartyId, string strBillPartyId, string strPurDate, string strBrokerId, string strPurPersonId, string strConvRate, string strPurTypeName, string strOrderNo, string strDueDays, string strDueDate, string strRefPurEmpId, ref string errMsg)
        ////{
        ////    String ret = "";
        ////    int x = 0;
        ////    try
        ////    {

        ////        List<string> queries = new List<string>();
        ////        string query1 = "";
        ////        string strDate = GlobalPara.LoginDate();
        ////        string strTime = GlobalPara.Logintime();

        ////        string strCertiNo = "";
        ////        string strLabType = "";
        ////        string strRoughType = "";
        ////        int iSrNo = 1;
        ////        string strIncription = "";
        ////        string strReportDate = "";

        ////        string strLabDate = "";
        ////        //string strOrderNo = "";
        ////        string strRemark = "";
        ////        string strShape = "";
        ////        string strWeight = "";
        ////        string strSym = "";
        ////        string strCut = "";
        ////        string strQuality = "";
        ////        string strColor = "";
        ////        string strMDisc = "";
        ////        string strMRate = "";
        ////        string strMAmount = "";
        ////        string strPolish = "";
        ////        string strShade = "";
        ////        string strHa = "";
        ////        string strFlor = "";

        ////        string strEye = "";

        ////        string strLuster = "";
        ////        string strMilky = "";
        ////        string strTable = "";
        ////        string strDepth = "";
        ////        string strRatio = "";
        ////        string strDiameter = "";
        ////        string strLength = "";
        ////        string strWidth = "";
        ////        string strHeight = "";

        ////        string strCrnHt = "";
        ////        string strPavHt = "";
        ////        string strCrnAg = "";
        ////        string strPavAg = "";
        ////        string strCulet = "";
        ////        string strGirdlePer = "";
        ////        string strGirdleDesc = "";

        ////        string strLrHalf = "";
        ////        string strLan = "";


        ////        string strKeyToSym = "";
        ////        string strLabRemarks = "";
        ////        string strLabShape = "";
        ////        string strWhiteTable = "";
        ////        string strWhiteTable1 = "";

        ////        string strWhiteSide = "";
        ////        string strWhiteSide1 = "";
        ////        string strBlackTable = "";
        ////        string strBlackSide = "";
        ////        string strOpenTable = "";
        ////        string strOpenSide = "";
        ////        string strOpenPav = "";
        ////        string strOpenGrid = "";
        ////        string strNt = "";

        ////        string strClientRef = "";
        ////        string strMemoNo = "";
        ////        string strPFace = "";
        ////        string strTsp = "";
        ////        string strSsp = "";
        ////        string strImgLink = "";
        ////        string strVideoLink = "";
        ////        string strRoughId = "";
        ////        string strStockDate = "";

        ////        //Raj's New Added
        ////        string strMColor = "";
        ////        string strMQuality = "";
        ////        string strMCut = "";
        ////        string strMPolish = "";
        ////        string strMSym = "";
        ////        string strMFlor = "";
        ////        string strRap = "0";
        ////        string strDisc = "0";
        ////        string strRate = "0";
        ////        string strAmount = "0";
        ////        string strLabRevisedDate = "";
        ////        string strRfidNo = "";
        ////        string strScsNO = "";
        ////        string strCustId = "";
        ////        string strLabName = "";
        ////        string strLabSite = "";
        ////        string strIsMovie = "0";
        ////        string strLocation = "";
        ////        string strMovieType = "";

        ////        if (strBrokerId == "" || strBrokerId == null)
        ////        {
        ////            strBrokerId = "null";
        ////        }

        ////        if (strRefPurEmpId == "" || strRefPurEmpId == null)
        ////        {
        ////            strRefPurEmpId = "null";
        ////        }

        ////        //Get Max InNO
        ////        string strInNo = "1";

        ////        string strSeqNo = General.getSeqNo("seq_seqno");
        ////        DataTable tblChk = new DataTable();
        ////        int cnt = 0;
        ////        DataTable tblOrdNo = new DataTable();

        ////        var tblColor = Masters.Instance.getRegularColor("", "name");
        ////        tblColor.CaseSensitive = false;

        ////        var tblQuality = Masters.Instance.getQuality("", "name");
        ////        tblQuality.CaseSensitive = false;

        ////        var tblSize = RapaPort.Instance.getSize("RAP");
        ////        tblSize.CaseSensitive = false;

        ////        var tblShape = Masters.Instance.getShape("disporder", "");
        ////        tblShape.CaseSensitive = false;

        ////        var tblCut = Masters.Instance.getCut("0", "disporder");
        ////        tblCut.CaseSensitive = false;

        ////        var tblFlo = Masters.Instance.getFlorotion("0", "disporder");
        ////        tblFlo.CaseSensitive = false;

        ////        var tblShade = Masters.Instance.getShade("0", "disporder");
        ////        tblShade.CaseSensitive = false;

        ////        var tblWhite = Masters.Instance.getInclusion("WHITE", "0", "disporder");
        ////        tblWhite.CaseSensitive = false;

        ////        var tblBlack = Masters.Instance.getInclusion("BLACK", "0", "disporder");
        ////        tblBlack.CaseSensitive = false;

        ////        var tblOpen = Masters.Instance.getInclusion("OPEN", "0", "disporder");
        ////        tblOpen.CaseSensitive = false;

        ////        var tblLabType = Masters.Instance.getLabType("0");
        ////        tblLabType.CaseSensitive = false;

        ////        var tblRoughType = Masters.Instance.getRoughType();
        ////        tblRoughType.CaseSensitive = false;

        ////        var tblPolish = Masters.Instance.getPolish();
        ////        tblPolish.CaseSensitive = false;

        ////        var tblSym = Masters.Instance.getSym();
        ////        tblSym.CaseSensitive = false;

        ////        var tblMilky = Masters.Instance.getMilky("0", "disporder");
        ////        tblMilky.CaseSensitive = false;

        ////        var tblLab = Masters.Instance.getLab("0", "username");
        ////        tblLab.CaseSensitive = false;

        ////        var tblLabSite = Masters.Instance.getLabLocation("0");
        ////        tblLabSite.CaseSensitive = false;

        ////        var tblLocation = Masters.Instance.getBranch();
        ////        tblLocation.CaseSensitive = false;

        ////        DataTable tblTmpData = new DataTable();
        ////        string strPacketId = "1";

        ////        int iStoneSeqno = 1;
        ////        string strStoneId = "";

        ////        string strIp = GlobalPara.RequestIp();
        ////        string strDVerId = "1";
        ////        string strHVerId = "1";
        ////        string strVerId = "1";
        ////        string strRoughNo = "";
        ////        string strOrdTranId = "";
        ////        string strLabShapeName = "";
        ////        string strCFace = "";
        ////        string strMfgRemarks = "";
        ////        string strPreRoughNo = "";
        ////        DataTable tblVer = General.ReturnTable("select rapid,max(tranid) as verid from version_history where is_approve='1' group by rapid");
        ////        double dRap = 0;
        ////        Double dTotWeight = 0;
        ////        int iPcs = 0;



        ////        DataRow[] drs;
        ////        DataTable tblLabNo = General.ReturnTable("select coalesce(max(labno+1),1) as labno from sale_data");
        ////        string strLabNo = tblLabNo.Rows[0]["labno"].ToString();

        ////        for (int i = 0; i <= tblVer.Rows.Count - 1; i++)
        ////        {
        ////            if (tblVer.Rows[i]["rapid"].ToString() == "1")
        ////            {
        ////                strDVerId = tblVer.Rows[i]["verid"].ToString();
        ////            }
        ////            else
        ////            {
        ////                strHVerId = tblVer.Rows[i]["verid"].ToString();
        ////            }
        ////        }
        ////        strVerId = strHVerId;


        ////        for (int i = 0; i <= tblData.Rows.Count - 1; i++)
        ////        {
        ////            if (i >= 139)
        ////            {
        ////                int z = 0;
        ////                z = z++;
        ////            }
        ////            strCertiNo = "null";
        ////            strLabType = "null";
        ////            strRoughType = "null";

        ////            strIncription = "null";
        ////            strReportDate = "null";

        ////            strLabDate = "null";
        ////            strOrdTranId = "0";
        ////            strRemark = "null";
        ////            strShape = "null";
        ////            strWeight = "null";
        ////            strSym = "null";
        ////            strCut = "null";
        ////            strQuality = "null";
        ////            strColor = "null";
        ////            strMDisc = "0";
        ////            strMRate = "0";
        ////            strMAmount = "0";
        ////            strPolish = "null";
        ////            strShade = "null";
        ////            strHa = "null";
        ////            strFlor = "null";

        ////            strEye = "0";

        ////            strLuster = "null";
        ////            strMilky = "null";
        ////            strTable = "null";
        ////            strDepth = "null";
        ////            strRatio = "null";
        ////            strDiameter = "null";
        ////            strLength = "null";
        ////            strWidth = "null";
        ////            strHeight = "null";

        ////            strCrnHt = "null";
        ////            strPavHt = "null";
        ////            strCrnAg = "null";
        ////            strPavAg = "null";
        ////            strCulet = "null";
        ////            strGirdlePer = "null";
        ////            strGirdleDesc = "null";

        ////            strLrHalf = "null";


        ////            strKeyToSym = "null";
        ////            strLabRemarks = "null";
        ////            strLabShape = "null";
        ////            strWhiteTable = "null";
        ////            strWhiteTable1 = "null";

        ////            strWhiteSide = "null";
        ////            strWhiteSide1 = "null";
        ////            strBlackTable = "null";
        ////            strBlackSide = "null";
        ////            strOpenTable = "null";
        ////            strOpenSide = "null";
        ////            strOpenPav = "null";
        ////            strOpenGrid = "null";
        ////            strNt = "null";

        ////            strClientRef = "null";
        ////            strMemoNo = "null";
        ////            strPFace = "null";
        ////            strTsp = "null";
        ////            strSsp = "null";
        ////            strImgLink = "null";
        ////            strVideoLink = "null";
        ////            strLabShapeName = "null";
        ////            strCFace = "";
        ////            strMfgRemarks = "";
        ////            strStockDate = "null";

        ////            //Raj's New Added
        ////            strMColor = "null";
        ////            strMQuality = "null";
        ////            strMCut = "null";
        ////            strMPolish = "null";
        ////            strMSym = "null";
        ////            strMFlor = "null";
        ////            strRap = "0";
        ////            strDisc = "0";
        ////            strRate = "0";
        ////            strAmount = "0";
        ////            strLabRevisedDate = "null";
        ////            strRfidNo = "null";
        ////            strScsNO = "null";
        ////            strCustId = "null";
        ////            strLabName = "null";
        ////            strLabSite = "null";
        ////            strIsMovie = "0";
        ////            strLocation = "null";
        ////            strMovieType = "";

        ////            if (tblData.Rows[i][1].ToString().Trim() == "" && tblData.Rows[i][4].ToString().Trim() == ""
        ////                && tblData.Rows[i][11].ToString().Trim() == "" && tblData.Rows[i][12].ToString().Trim() == ""
        ////                && tblData.Rows[i][13].ToString().Trim() == "" && tblData.Rows[i][14].ToString().Trim() == "")
        ////            {
        ////                ret = "Error";
        ////                errMsg = "Please Check data Line no " + cnt;
        ////                return ret;
        ////            }


        ////            if (tblData.Rows[i][3].ToString().Trim() != "")
        ////            {
        ////                drs = tblLabType.Select("name='" + tblData.Rows[i][3].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strLabType = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][4].ToString().Trim() != "")
        ////            {
        ////                drs = tblRoughType.Select("name='" + tblData.Rows[i][4].ToString().Trim() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strRoughType = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }

        ////            strCertiNo = "'" + tblData.Rows[i][5].ToString().Trim() + "'";
        ////            strIncription = "'" + tblData.Rows[i][6].ToString().Trim() + "'";

        ////            if (tblData.Rows[i][7].ToString().Trim() != "")
        ////            {
        ////                strReportDate = "'" + Convert.ToDateTime(tblData.Rows[i][7]).ToString("yyyy-MM-dd") + "'";
        ////            }
        ////            if (tblData.Rows[i][8].ToString().Trim() != "")
        ////            {
        ////                strLabDate = "'" + Convert.ToDateTime(tblData.Rows[i][8]).ToString("yyyy-MM-dd") + "'";

        ////            }

        ////            //Order No 
        ////            if (tblData.Rows[i][9].ToString().Trim() != "")
        ////            {

        ////            }

        ////            strRemark = General.ReplcaeQuotString(tblData.Rows[i][10].ToString());
        ////            strLabShapeName = tblData.Rows[i][11].ToString().Trim();

        ////            if (tblData.Rows[i][11].ToString().Trim() != "")
        ////            {
        ////                drs = tblShape.Select("mig_code='" + tblData.Rows[i][11].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable(); if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strShape = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            strWeight = tblData.Rows[i][12].ToString().Trim();
        ////            dTotWeight = dTotWeight + Convert.ToDouble(tblData.Rows[i][12]);

        ////            if (tblData.Rows[i][13].ToString().Trim() != "")
        ////            {
        ////                drs = tblColor.Select("name='" + tblData.Rows[i][13].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strColor = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][14].ToString().Trim() != "")
        ////            {
        ////                drs = tblQuality.Select("name='" + tblData.Rows[i][14].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strQuality = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }

        ////            if (tblData.Rows[i][15].ToString().Trim() != "")
        ////            {
        ////                strMDisc = tblData.Rows[i][15].ToString().Trim();

        ////            }
        ////            if (tblData.Rows[i][16].ToString().Trim() != "")
        ////            {
        ////                strMRate = tblData.Rows[i][16].ToString().Trim();

        ////            }
        ////            if (tblData.Rows[i][17].ToString().Trim() != "")
        ////            {
        ////                strMAmount = tblData.Rows[i][17].ToString().Trim();

        ////            }
        ////            if (tblData.Rows[i][18].ToString().Trim() != "")
        ////            {
        ////                if (tblData.Rows[i][18].ToString().Trim().ToUpper() == "ID")
        ////                {
        ////                    tblData.Rows[i][18] = "ID";
        ////                }
        ////                tblTmpData = tblCut.Select("name='" + tblData.Rows[i][18].ToString().Trim() + "'", "").CopyToDataTable();
        ////                if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                {
        ////                    strCut = tblTmpData.Rows[0]["tranid"].ToString();
        ////                }
        ////            }
        ////            //Polish
        ////            if (tblData.Rows[i][19].ToString().Trim() != "")
        ////            {
        ////                drs = tblPolish.Select("name='" + tblData.Rows[i][19].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strPolish = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            //Sym
        ////            if (tblData.Rows[i][20].ToString().Trim() != "")
        ////            {
        ////                drs = tblSym.Select("name='" + tblData.Rows[i][20].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strSym = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][21].ToString().Trim() != "")
        ////            {
        ////                drs = tblFlo.Select("name='" + tblData.Rows[i][21].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strFlor = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }

        ////            if (tblData.Rows[i][22].ToString().Trim() != "")
        ////            {
        ////                drs = tblShade.Select("name='" + tblData.Rows[i][22].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strShade = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            strHa = "'" + tblData.Rows[i][23].ToString().Trim() + "'";
        ////            if (tblData.Rows[i][24].ToString().Trim().ToUpper() == "EYECLEAN")
        ////            {
        ////                strEye = "1";
        ////            }
        ////            strLuster = "'" + tblData.Rows[i][25].ToString().Trim() + "'";

        ////            if (tblData.Rows[i][26].ToString().Trim() != "")
        ////            {
        ////                drs = tblMilky.Select("name='" + tblData.Rows[i][26].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strMilky = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            strTable = "'" + tblData.Rows[i][27].ToString().Trim() + "'";
        ////            strDepth = "'" + tblData.Rows[i][28].ToString().Trim() + "'";

        ////            //Ratio Pending
        ////            if (tblData.Rows[i][29].ToString().Trim() != "")
        ////            {

        ////            }
        ////            strLength = "'" + tblData.Rows[i][31].ToString().Trim() + "'";
        ////            strWidth = "'" + tblData.Rows[i][32].ToString().Trim() + "'";
        ////            strHeight = "'" + tblData.Rows[i][33].ToString().Trim() + "'";

        ////            strDiameter = ((Convert.ToDouble(tblData.Rows[i][31].ToString().Trim()) + Convert.ToDouble(tblData.Rows[i][32].ToString().Trim())) / 2).ToString();

        ////            strCrnAg = "'" + tblData.Rows[i][34].ToString().Trim() + "'";
        ////            strCrnHt = "'" + tblData.Rows[i][35].ToString().Trim() + "'";
        ////            strPavAg = "'" + tblData.Rows[i][36].ToString().Trim() + "'";
        ////            strPavHt = "'" + tblData.Rows[i][37].ToString().Trim() + "'";
        ////            strGirdlePer = "'" + tblData.Rows[i][38].ToString().Trim() + "'";

        ////            strLan = "'" + tblData.Rows[i][39].ToString().Trim() + "'";
        ////            strLrHalf = "'" + tblData.Rows[i][40].ToString().Trim() + "'";
        ////            strGirdleDesc = "'" + tblData.Rows[i][41].ToString().Trim() + "'";
        ////            strCulet = "'" + tblData.Rows[i][42].ToString().Trim() + "'";
        ////            strKeyToSym = "'" + tblData.Rows[i][43].ToString().Trim() + "'";
        ////            strLabRemarks = tblData.Rows[i][44].ToString().Trim();

        ////            strLabShape = strShape;
        ////            if (tblData.Rows[i][46].ToString().Trim() != "")
        ////            {
        ////                drs = tblWhite.Select("name='" + tblData.Rows[i][46].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strWhiteTable1 = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                    if (strWhiteTable1 == "5")
        ////                    {
        ////                        strWhiteTable = "5";
        ////                    }
        ////                    else if (strWhiteTable1 == "25" || strWhiteTable1 == "26")
        ////                    {
        ////                        strWhiteTable = "6";
        ////                    }
        ////                    else if (strWhiteTable1 == "23" || strWhiteTable1 == "24")
        ////                    {
        ////                        strWhiteTable = "7";
        ////                    }
        ////                    else if (strWhiteTable1 == "20" || strWhiteTable1 == "21" || strWhiteTable1 == "22")
        ////                    {
        ////                        strWhiteTable = "8";
        ////                    }
        ////                    else if (strWhiteTable1 == "17" || strWhiteTable1 == "18" || strWhiteTable1 == "19")
        ////                    {
        ////                        strWhiteTable = "9";
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][47].ToString().Trim() != "")
        ////            {
        ////                drs = tblWhite.Select("name='" + tblData.Rows[i][47].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strWhiteSide1 = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                    if (strWhiteSide1 == "5")
        ////                    {
        ////                        strWhiteSide = "5";
        ////                    }
        ////                    else if (strWhiteSide1 == "25" || strWhiteSide1 == "26")
        ////                    {
        ////                        strWhiteSide = "6";
        ////                    }
        ////                    else if (strWhiteSide1 == "23" || strWhiteSide1 == "24")
        ////                    {
        ////                        strWhiteSide = "7";
        ////                    }
        ////                    else if (strWhiteSide1 == "20" || strWhiteSide1 == "21" || strWhiteSide1 == "22")
        ////                    {
        ////                        strWhiteSide = "8";
        ////                    }
        ////                    else if (strWhiteSide1 == "17" || strWhiteSide1 == "18" || strWhiteSide1 == "19")
        ////                    {
        ////                        strWhiteSide = "9";
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][48].ToString().Trim() != "")
        ////            {
        ////                drs = tblBlack.Select("name='" + tblData.Rows[i][48].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strBlackTable = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][49].ToString().Trim() != "")
        ////            {
        ////                drs = tblBlack.Select("name='" + tblData.Rows[i][49].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strBlackSide = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][50].ToString().Trim() != "")
        ////            {
        ////                drs = tblOpen.Select("name='" + tblData.Rows[i][50].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strOpenTable = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][51].ToString().Trim() != "")
        ////            {
        ////                drs = tblOpen.Select("name='" + tblData.Rows[i][51].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strOpenSide = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][52].ToString().Trim() != "")
        ////            {
        ////                drs = tblOpen.Select("name='" + tblData.Rows[i][52].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strOpenPav = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][53].ToString().Trim() != "")
        ////            {
        ////                drs = tblOpen.Select("name='" + tblData.Rows[i][53].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strOpenGrid = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            strNt = "'" + tblData.Rows[i][54].ToString().Trim() + "'";

        ////            strPFace = "'" + tblData.Rows[i][55].ToString().Trim() + "'";
        ////            strTsp = "'" + tblData.Rows[i][56].ToString().Trim() + "'";
        ////            strSsp = "'" + tblData.Rows[i][57].ToString().Trim() + "'";
        ////            strImgLink = "'" + tblData.Rows[i][58].ToString().Trim() + "'";
        ////            strVideoLink = "'" + tblData.Rows[i][59].ToString().Trim() + "'";
        ////            strStoneId = tblData.Rows[i][60].ToString().Trim();
        ////            dRap = Convert.ToDouble(tblData.Rows[i][61].ToString().Trim());
        ////            strMfgRemarks = tblData.Rows[i][62].ToString().Trim();
        ////            strCFace = tblData.Rows[i][63].ToString().Trim();

        ////            if (tblData.Rows[i][64].ToString().Trim() != "")
        ////            {
        ////                strStockDate = "'" + Convert.ToDateTime(tblData.Rows[i][64]).ToString("yyyy-MM-dd") + "'";
        ////            }

        ////            //Raj's New Added
        ////            if (tblData.Rows[i][5].ToString().Trim() != "")//check certiNo
        ////            {
        ////                strPurType = "58";
        ////                strPurTypeName = "Available";
        ////            }
        ////            else
        ////            {
        ////                strPurType = "53";
        ////                strPurTypeName = "New_Arrival";
        ////            }

        ////            if (tblData.Rows[i][65].ToString().Trim() != "")
        ////            {
        ////                drs = tblColor.Select("name='" + tblData.Rows[i][65].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strMColor = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][66].ToString().Trim() != "")
        ////            {
        ////                drs = tblQuality.Select("name='" + tblData.Rows[i][66].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strMQuality = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][67].ToString().Trim() != "")
        ////            {
        ////                if (tblData.Rows[i][67].ToString().Trim().ToUpper() == "ID")
        ////                {
        ////                    tblData.Rows[i][67] = "ID";
        ////                }
        ////                tblTmpData = tblCut.Select("name='" + tblData.Rows[i][67].ToString().Trim() + "'", "").CopyToDataTable();
        ////                if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                {
        ////                    strMCut = tblTmpData.Rows[0]["tranid"].ToString();
        ////                }
        ////            }
        ////            if (tblData.Rows[i][68].ToString().Trim() != "")
        ////            {
        ////                drs = tblPolish.Select("name='" + tblData.Rows[i][68].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strMPolish = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][69].ToString().Trim() != "")
        ////            {
        ////                drs = tblSym.Select("name='" + tblData.Rows[i][69].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strMSym = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][70].ToString().Trim() != "")
        ////            {
        ////                drs = tblFlo.Select("name='" + tblData.Rows[i][70].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strMFlor = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][71].ToString().Trim() != "")
        ////            {
        ////                strRap = tblData.Rows[i][71].ToString().Trim();
        ////            }
        ////            if (tblData.Rows[i][72].ToString().Trim() != "")
        ////            {
        ////                strDisc = tblData.Rows[i][72].ToString().Trim();
        ////            }
        ////            if (tblData.Rows[i][73].ToString().Trim() != "")
        ////            {
        ////                strRate = tblData.Rows[i][73].ToString().Trim();
        ////            }
        ////            if (tblData.Rows[i][74].ToString().Trim() != "")
        ////            {
        ////                strAmount = tblData.Rows[i][74].ToString().Trim();
        ////            }
        ////            //l.revised date
        ////            if (tblData.Rows[i][75].ToString().Trim() != "")
        ////            {
        ////                strLabRevisedDate = "'" + Convert.ToDateTime(tblData.Rows[i][75]).ToString("yyyy-MM-dd") + "'";
        ////            }
        ////            strRfidNo = tblData.Rows[i][76].ToString().Trim();
        ////            //order name 77
        ////            if (tblData.Rows[i][77].ToString().Trim() != "")
        ////            {
        ////                strOrderNo = tblData.Rows[i][77].ToString().Trim();

        ////                tblOrdNo = General.ReturnTable("select * from order_data where trim(upper(name))='" + strOrderNo.ToUpper() + "' ");

        ////                if (tblOrdNo != null && tblOrdNo.Rows.Count > 0)
        ////                {
        ////                    strOrdTranId = tblOrdNo.Rows[0]["tranid"].ToString();

        ////                }
        ////            }

        ////            if (tblData.Rows[i][78].ToString().Trim() != "")
        ////            {
        ////                drs = tblLab.Select("name='" + tblData.Rows[i][78].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strLabName = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }
        ////            if (tblData.Rows[i][79].ToString().Trim() != "")
        ////            {
        ////                drs = tblLabSite.Select("name='" + tblData.Rows[i][79].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strLabSite = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }

        ////            if (tblData.Rows[i][80].ToString().Trim() != "")
        ////            {
        ////                strIsMovie = "1";
        ////                strMovieType = tblData.Rows[i][80].ToString().Trim();
        ////            }
        ////            //location 81
        ////            if (tblData.Rows[i][81].ToString().Trim() != "")
        ////            {
        ////                drs = tblLocation.Select("name='" + tblData.Rows[i][81].ToString().Trim().ToUpper() + "'", "");
        ////                if (drs.Length > 0)
        ////                {
        ////                    tblTmpData = drs.CopyToDataTable();
        ////                    if (tblTmpData != null && tblTmpData.Rows.Count > 0)
        ////                    {
        ////                        strLocation = tblTmpData.Rows[0]["tranid"].ToString();
        ////                    }
        ////                }
        ////            }

        ////            strScsNO = tblData.Rows[i][82].ToString().Trim();
        ////            strCustId = tblData.Rows[i][83].ToString().Trim();


        ////            //tblOrdNo = General.ReturnTable("select * from po_data where ordno='" + strOrderNo + "' and shapeid='" + strShape + "' and " + strWeight + " between f_size and t_size ");

        ////            //if (tblOrdNo != null && tblOrdNo.Rows.Count > 0)
        ////            //{
        ////            //    strOrdTranId = tblOrdNo.Rows[0]["tranid"].ToString();

        ////            //    query1 = " update po_data set pending_pcs = pending_pcs - 1 , comp_pcs = comp_pcs + 1 where tranid=" + strOrdTranId;
        ////            //    queries.Add(query1);

        ////            //}


        ////            //Check RoughNo.if exist then get ExistNo else Add New Rough
        ////            //if (cnt == 0)
        ////            if (strPreRoughNo != tblData.Rows[i][1].ToString().Substring(1))
        ////            {
        ////                if (cnt > 1)
        ////                {
        ////                    iStoneSeqno = iStoneSeqno - 1;
        ////                    iPcs = iPcs - 1;
        ////                    dTotWeight = dTotWeight - Convert.ToDouble(tblData.Rows[i][12]);
        ////                    query1 = "update m_rough set pcs= coalesce(pcs,0) + '" + iPcs + "',weight = coalesce(weight,0) + '" + dTotWeight + "',stone_seqno=" + iStoneSeqno + " where tranid='" + strRoughId + "' ";
        ////                    queries.Add(query1);
        ////                    iStoneSeqno = 0;
        ////                    iPcs = 0;
        ////                    dTotWeight = Convert.ToDouble(tblData.Rows[i][12]);

        ////                }
        ////                tblChk = General.ReturnTable("select rpad(replace(translate(lower(rough_no),'abcdefghijklmnopqrstuvwxyz',rpad('z',26,'z')),'z',''),3,'0')  as rough_no,'83' as stno,lpad((stone_seqno+1)::text,4,'0') as stone_seqno,* from m_rough where upper(trim(rough_no))='" + tblData.Rows[i][1].ToString().Replace("'", "").Trim().ToUpper() + "'");
        ////                if (tblChk != null && tblChk.Rows.Count > 0)
        ////                {
        ////                    //strStoneId = tblChk.Rows[0]["stno"].ToString() + tblChk.Rows[0]["rough_no"].ToString() + tblChk.Rows[0]["stone_seqno"].ToString();
        ////                    iStoneSeqno = Convert.ToInt32(tblChk.Rows[0]["stone_seqno"]);
        ////                    strRoughNo = tblChk.Rows[0]["rough_no"].ToString();

        ////                    strRoughId = tblChk.Rows[0]["tranid"].ToString();
        ////                    tblChk = General.ReturnTable(" select COALESCE(MAX(srno) + 1, 1) as srno from m_packet where roughid='" + tblChk.Rows[0]["tranid"].ToString() + "' ");
        ////                    if (tblChk != null && tblChk.Rows.Count > 0)
        ////                    {
        ////                        iSrNo = Convert.ToInt32(tblChk.Rows[0]["srno"]);
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    strRoughId = General.getSeqNo("m_rough_tranid_seq");
        ////                    //Get Max Purchase RoughNo
        ////                    strRoughNo = tblData.Rows[i][1].ToString().Substring(1);
        ////                    //strRoughNo = strRoughNo.PadLeft(4 - strRoughNo.Length, '0');
        ////                    //strStoneId = "83" + strRoughNo + "".PadRight(3 - strRoughNo.Length, '0') + "".ToString().PadRight(4 - iStoneSeqno.ToString().Length, '0') + iStoneSeqno;

        ////                    query1 = " INSERT INTO public.m_rough(tranid,rough_typeid,originid,rap_typeid,name,invoice_date,receive_date, labour_type, weight,  size, rate, dollar_rate, amount, dollar_amount, brokerage_per, status, brokrege_amount, c_year, addid, adddate, addtime, labour_amount, partyid, brokerageid, authid, authdate," +
        ////                             " authtime, rough_no, branchid, deptid, sectionid, isassort, pkt_pending_pcs, cur_deptid, cur_empid, cur_status, cur_date, cur_time, priorityid, is_crosschk_rough, stone_seqno) " +
        ////                             " values ('" + strRoughId + "','" + strRoughType + "','1','2','" + tblData.Rows[i][1].ToString().Trim().Replace("'", "") + "','" + strDate + "','" + strDate + "','','0','0','" + strConvRate + "','" + strConvRate + "','0','0','0','" + strPurType + "','0','" + GlobalPara.CouponYear + "','" + GlobalPara.CurrentUserId + "','" + strDate + "','" + strTime + "','0','" + strPartyId + "','0','" + GlobalPara.CurrentUserId + "','" + strDate +
        ////                             "','" + strTime + "','" + tblData.Rows[i][1].ToString().Trim().Replace("'", "") + "','1','20','20','0','0','20','1','" + strPurType + "','" + strDate + "','" + strTime + "','1','0','0')";
        ////                    queries.Add(query1);

        ////                }

        ////                strPreRoughNo = tblData.Rows[i][1].ToString().Substring(1);
        ////            }
        ////            iPcs++;
        ////            cnt++;
        ////            strPacketId = General.getSeqNo("m_packet_tranid_seq");
        ////            if (tblData.Rows[i][2].ToString().Trim() != "")
        ////            {
        ////                iSrNo = Convert.ToInt32(tblData.Rows[i][2]);
        ////            }

        ////            query1 = " INSERT INTO m_packet(tranid, roughid, weight, pcs, colorid, florid, qualityid, tensonid, length, width, height, srno, tag, cur_deptid, cur_empid, cur_date, cur_time, status, stock_deptid, janno, addid, adddate, addtime, processid, rec_pcs, rec_weight, iscomplete, comp_date, islock, seqno, org_weight, is_pkt_iss, is_prediction, is_crosschk, is_checking, is_report, is_pred_done, is_final_makable, makable_date, makable_time, exp_weight, shapeid, is_marker_stock, is_hpred_done, repair_cnt,  mfg_colorid, mfg_shapeid, mfg_qualityid, makable_weight, is_mfg_transfer, status_flgid, mfg_cutid, stoneid, labid, certino, is_finalchecking, is_process_lock,  orderid, order_unlock, certi_date, is_barcode_print,ispurchase,roughtypeid,img_path,video_path,is_movie,movie_type,rfid) " +
        ////                     " values('" + strPacketId + "', '" + strRoughId + "', '" + strWeight + "', '1', '" + strColor + "', " + strFlor + ", '" + strQuality + "', '1', " + strLength + "," + strWidth + "," + strHeight + ",'" + iSrNo + "', 'A', '20', '1', '" + strDate + "', '" + strTime + "', '60', '20', '1', '" + GlobalPara.CurrentUserId + "', '" + strDate + "','" + strTime + "', '1', '1', '" + strWeight + "', '1', '" + strDate + "','0', '" + strSeqNo + "', '" + strWeight + "', '1', '1', '1', '1', '0', '1', '1', '" + strDate + "', '" + strTime + "', '" + strWeight + "','" + strShape + "', '0', '1', '0','" + strColor + "'," + strLabShape + ",'" + strQuality + "','" + strWeight + "', '1', '1'," + strCut + ", '" + strStoneId + "', '1'," + strCertiNo +
        ////                     ", '1', '0',  '" + strOrdTranId + "', '0', " + strReportDate + ",'1','1','" + strRoughType + "'," + strImgLink + "," + strVideoLink + ",'" + strIsMovie + "','" + strMovieType + "','" + strRfidNo + "') ";

        ////            queries.Add(query1);


        ////            //query1 = " INSERT INTO public.sale_data(roughid, packetid, stoneid, status, status_flgid, ispurchase, is_conf_pen, iss_pcs, iss_weight, iss_deptid, iss_empid, iss_date, iss_time, rec_pcs, rec_weight, rec_deptid, rec_empid, rec_date, rec_time, iss_ip, rec_ip, addid, adddate, addtime, conid, condate, contime, inno, recno, janno, con_ip, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, gcolorid, gqualid, gcutid, gpolid, gsymid, gflorid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, tensonid, orderid,  stock_deptid, branchid, stock_empid, stock_date, stock_time, pkt, rtype, roughtypeid, versionid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, paface, culet, owin, luster, cro, pav, sta, lhl, gir, cro_per, pav_per, certino, avg_dia, max_dia, min_dia, height, d, t, is_eye, ord, is_movie, gidate, gitime, shadeid1, whitetableid1, whitesideid1, orgrap, perdis,  rate, amount,  gorgrap, gperdis, grate, gamount, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, " +
        ////            //         " lab_typeid, porgrap, pperdis, prate, pamount, price_authid,auth_rate, auth_perdis, auth_amount, lab_iss_remarks, pricer_remarks, keytosym, insc, lab_rec_remarks, certi_type, corgrap, cperdis, crate, camount, dorgrap, dperdis, drate, damount, packet_entry_remarks, gir_desc, brokerid, select_lab_typeid, sel_colorid, sel_qualid, statusname,purno,lab_shapename,labno,cface,remark,img_path,video_path,certi_date)" +
        ////            //        " values('" + strRoughId + "','" + strPacketId + "','" + strStoneId + "','" + strPurType + "', '1', '1', '0', '1', '" + strWeight + "', '20', '1', '" + strDate + "', '" + strTime + "', '1', '" + strWeight + "', '20', '1', '" + strDate + "', '" + strTime + "', '" + strIp + "', '" + strIp + "', '" + GlobalPara.CurrentUserId +
        ////            //        "', '" + strDate + "', '" + strTime + "', '" + GlobalPara.CurrentUserId + "', '" + strDate + "', '" + strTime + "', '" + strInNo + "', '0', '" + strInNo + "', '" + strIp + "', '" + strShape + "', '" + strColor + "', " + strQuality + ", " + strCut + ", " + strPolish + ", " + strSym +
        ////            //        ", " + strFlor + ", " + strLabShape + ", " + strColor + ", " + strQuality + ", " + strCut + ", " + strPolish + ", " + strSym + ", " + strFlor + ", " + strWhiteTable + ", " + strWhiteSide +
        ////            //        ", " + strBlackTable + ", " + strBlackSide + ", " + strOpenTable + ", " + strOpenSide + ", " + strShade + ", '1', '0',  '20', '1', '1', " + strStockDate + ", '" + strTime + "', 'Sin', 'S', " + strRoughType + ", " + strVerId + ", " + strOpenPav + ", " + strOpenGrid +
        ////            //        ", " + strMilky + ", " + strTsp + ", " + strSsp + ", " + strNt + ", " + strHa + ", " + strPFace + ", " + strCulet + ", '', " + strLuster + ", " + strCrnAg + ", " + strPavAg + ", " + strLan + ", " + strLrHalf +
        ////            //        ", " + strGirdlePer + ", " + strCrnHt + ", " + strPavHt + ", " + strCertiNo + ", '" + strDiameter + "', " + strLength + ", " + strWidth + ", " + strHeight + ", " + strDepth +
        ////            //        ", " + strTable + ", " + strEye + ", '0', '0', '" + strDate + "', '" + strTime + "', " + strShade + ", " + strWhiteTable1 + ", " + strWhiteSide1 + ", '" + dRap + "', '" + strMDisc + "',  '" + strMRate + "', '" + strMAmount + "',  '" + dRap + "', '" + strMDisc + "',  '" + strMRate + "', '" + strMAmount + "', " + strLabDate + ", '" + strTime + "', " + strLabDate + ", '" + strTime + "', '" + GlobalPara.CurrentUserId + "', '" + GlobalPara.CurrentUserId +
        ////            //        "', " + strLabType + ", '0', '0',  '0', '0', '1', '0',  '0', '0', '', '', " + strKeyToSym +
        ////            //        ", " + strIncription + ", '" + strLabRemarks + "', '" + tblData.Rows[i][3].ToString() + "', '" + dRap + "', '" + strMDisc + "',  '" + strMRate + "', '" + strMAmount + "', '0', '0',  '0', '0', '" + strRemark + "', " + strGirdleDesc + ", null, " + strLabType + ", '" + strColor + "', '" + strQuality + "', '" + strPurTypeName + "','" + strInNo + "','" + General.ReplcaeQuotString(strLabShapeName) +
        ////            //        "','" + strLabNo + "','" + strCFace + "','" + strMfgRemarks + "'," + strImgLink + "," + strVideoLink + "," + strReportDate + ")";

        ////            //queries.Add(query1);

        ////            //query1 = " INSERT INTO public.sale_history(roughid, packetid, stoneid, status, status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, janname, jandate, lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, mfg_remarks, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno, memono, export_partyid, sale_janno, statusname,  invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,iss_ip,certi_date,memo_name,lab_shapename) " +
        ////            //         " select roughid, packetid, stoneid, '" + strPurType + "', status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, sale_janname, '" + strDate + "', lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, remark, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno,  memono, export_partyid, sale_janno, statusname, invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,'" + GlobalPara.RequestIp() + "',certi_date,memo_name,lab_shapename from sale_data where stoneid='" + strStoneId + "' ";

        ////            //queries.Add(query1);

        ////            if (strPurType == "58")
        ////            {
        ////                query1 = " INSERT INTO public.sale_data(roughid, packetid, stoneid, status, status_flgid, ispurchase, is_conf_pen, iss_pcs, iss_weight,  " +
        ////                    " iss_deptid, iss_empid, iss_date, iss_time, rec_pcs, rec_weight, rec_deptid, rec_empid, rec_date, rec_time, iss_ip, rec_ip, " +
        ////                    " addid, adddate, addtime, conid, condate, contime,inno, recno, janno, con_ip,  " +
        ////                    " shapeid, colorid, qualid, cutid, polid, symid, florid,  " +
        ////                    " gshapeid, gcolorid, gqualid, gcutid, gpolid, gsymid, gflorid,  " +
        ////                    " whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid,  " +
        ////                    " tensonid, orderid,  stock_deptid, branchid, stock_empid, stock_date, stock_time, pkt, rtype, roughtypeid, versionid, openpavid, opengridid,  " +
        ////                    " milkyid, tsp, ssp, nat, ha, paface, culet, owin, luster,  " +
        ////                    " cro, pav, sta, lhl, gir, cro_per, pav_per, " +
        ////                    " certino, avg_dia, max_dia, min_dia, height, d, t, is_eye,  " +
        ////                    " ord, is_movie, gidate, gitime, shadeid1, whitetableid1, whitesideid1,  " +
        ////                    " orgrap, perdis,  rate, amount,  " +
        ////                    " gorgrap, gperdis, grate, gamount,  " +
        ////                    " lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, " +
        ////                    " lab_typeid, porgrap, pperdis, prate, pamount, price_authid,auth_rate, auth_perdis, auth_amount, lab_iss_remarks, pricer_remarks, " +
        ////                    " keytosym, insc, lab_rec_remarks, certi_type, " +
        ////                    " corgrap, cperdis, crate, camount,  " +
        ////                    " dorgrap, dperdis, drate, damount,  " +
        ////                    " packet_entry_remarks, gir_desc, brokerid, select_lab_typeid, sel_colorid, sel_qualid, " +
        ////                    " statusname,purno,lab_shapename,labno,cface,remark, " +
        ////                    " img_path,video_path,certi_date,price_revised_date,labid,lab_locid,scsno,rfid,custid)" +

        ////                    " values('" + strRoughId + "','" + strPacketId + "','" + strStoneId + "','" + strPurType + "', '1', '1', '0', '1', '" + strWeight + "' " +
        ////                    ",'20', '1', '" + strDate + "', '" + strTime + "', '1', '" + strWeight + "', '20', '1', '" + strDate + "', '" + strTime + "', '" + strIp + "', '" + strIp + "' " +
        ////                    ",'" + GlobalPara.CurrentUserId + "', '" + strDate + "', '" + strTime + "', '" + GlobalPara.CurrentUserId + "', '" + strDate + "', '" + strTime + "','" + strInNo + "', '0', '" + strInNo + "', '" + strIp + "' " +
        ////                    ",'" + strShape + "', '" + strMColor + "', " + strMQuality + ", " + strMCut + ", " + strMPolish + ", " + strMSym + ", " + strMFlor + " " +
        ////                    "," + strLabShape + ", " + strColor + ", " + strQuality + ", " + strCut + ", " + strPolish + ", " + strSym + ", " + strFlor + " " +
        ////                    ", " + strWhiteTable + ", " + strWhiteSide + ", " + strBlackTable + ", " + strBlackSide + ", " + strOpenTable + ", " + strOpenSide + ", " + strShade + " " +
        ////                    ",'1', '" + strOrdTranId + "',  '20', '" + strLocation + "', '1', " + strStockDate + ", '" + strTime + "', 'Sin', 'S', " + strRoughType + ", " + strVerId + ", " + strOpenPav + ", " + strOpenGrid +
        ////                    ", " + strMilky + ", " + strTsp + ", " + strSsp + ", " + strNt + ", " + strHa + ", " + strPFace + ", " + strCulet + ", '', " + strLuster + " " +
        ////                    "," + strCrnAg + ", " + strPavAg + ", " + strLan + ", " + strLrHalf + ", " + strGirdlePer + ", " + strCrnHt + ", " + strPavHt + " " +
        ////                    "," + strCertiNo + ", '" + strDiameter + "', " + strLength + ", " + strWidth + ", " + strHeight + ", " + strDepth + ", " + strTable + ", " + strEye + " " +
        ////                    ",'0', '" + strIsMovie + "', '" + strDate + "', '" + strTime + "', " + strShade + ", " + strWhiteTable1 + ", " + strWhiteSide1 + " " +
        ////                    ", '" + strRap + "', '" + strDisc + "',  '" + strRate + "', '" + strAmount + "' " +
        ////                    ",'" + dRap + "', '" + strMDisc + "',  '" + strMRate + "', '" + strMAmount + "' " +
        ////                    ", " + strLabDate + ", '" + strTime + "', '" + strDate + "', '" + strTime + "', '" + GlobalPara.CurrentUserId + "', '" + GlobalPara.CurrentUserId + "' " +
        ////                    ", " + strLabType + ", '" + dRap + "', '" + strMDisc + "',  '" + strMRate + "', '" + strMAmount + "', '1', '" + strMRate + "',  '" + strMDisc + "', '" + strMAmount + "', '', '' " +
        ////                    ", " + strKeyToSym + ", " + strIncription + ", '" + strLabRemarks + "', '" + tblData.Rows[i][3].ToString() + "' " +
        ////                    ",'" + dRap + "', '" + strMDisc + "',  '" + strMRate + "', '" + strMAmount + "' " +
        ////                    ",'" + dRap + "', '" + strMDisc + "',  '" + strMRate + "', '" + strMAmount + "' " +
        ////                    ",'" + strRemark + "', " + strGirdleDesc + ", null, " + strLabType + ", '" + strColor + "', '" + strQuality + "' " +
        ////                    ", '" + strPurTypeName + "','" + strInNo + "','" + General.ReplcaeQuotString(strLabShapeName) + "','" + strLabNo + "','" + strCFace + "','" + strMfgRemarks + "' " +
        ////                    "," + strImgLink + "," + strVideoLink + "," + strReportDate + "," + strLabRevisedDate + "," + strLabName + "," + strLabSite + ",'" + strScsNO + "','" + strRfidNo + "','" + strCustId + "')";

        ////                queries.Add(query1);
        ////            }
        ////            else
        ////            {
        ////                query1 = " INSERT INTO public.sale_data(roughid, packetid, stoneid, status, status_flgid, ispurchase, is_conf_pen, iss_pcs, iss_weight,  " +
        ////                    " iss_deptid, iss_empid, iss_date, iss_time, rec_pcs, rec_weight, rec_deptid, rec_empid, rec_date, rec_time, iss_ip, rec_ip, " +
        ////                    " addid, adddate, addtime, conid, condate, contime,inno, recno, janno, con_ip,  " +
        ////                    " shapeid, colorid, qualid, cutid, polid, symid, florid,  " +
        ////                    " gshapeid, gcolorid, gqualid, gcutid, gpolid, gsymid, gflorid,  " +
        ////                    " whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid,  " +
        ////                    " tensonid, orderid,  stock_deptid, branchid, stock_empid, stock_date, stock_time, pkt, rtype, roughtypeid, versionid, openpavid, opengridid,  " +
        ////                    " milkyid, tsp, ssp, nat, ha, paface, culet, owin, luster,  " +
        ////                    " cro, pav, sta, lhl, gir, cro_per, pav_per, " +
        ////                    " certino, avg_dia, max_dia, min_dia, height, d, t, is_eye,  " +
        ////                    " ord, is_movie, gidate, gitime, shadeid1, whitetableid1, whitesideid1,  " +
        ////                    " orgrap, perdis,  rate, amount,  " +
        ////                    " gorgrap, gperdis, grate, gamount,  " +
        ////                    " lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, " +
        ////                    " lab_typeid, porgrap, pperdis, prate, pamount, price_authid,auth_rate, auth_perdis, auth_amount, lab_iss_remarks, pricer_remarks, " +
        ////                    " keytosym, insc, lab_rec_remarks, certi_type, " +
        ////                    " corgrap, cperdis, crate, camount,  " +
        ////                    " dorgrap, dperdis, drate, damount,  " +
        ////                    " packet_entry_remarks, gir_desc, brokerid, select_lab_typeid, sel_colorid, sel_qualid, " +
        ////                    " statusname,purno,lab_shapename,labno,cface,remark, " +
        ////                    " img_path,video_path,certi_date,price_revised_date,labid,lab_locid,scsno,rfid,custid)" +

        ////                    " values('" + strRoughId + "','" + strPacketId + "','" + strStoneId + "','" + strPurType + "', '1', '1', '0', '1', '" + strWeight + "' " +
        ////                    ",'20', '1', '" + strDate + "', '" + strTime + "', '1', '" + strWeight + "', '20', '1', '" + strDate + "', '" + strTime + "', '" + strIp + "', '" + strIp + "' " +
        ////                    ",'" + GlobalPara.CurrentUserId + "', '" + strDate + "', '" + strTime + "', '" + GlobalPara.CurrentUserId + "', '" + strDate + "', '" + strTime + "','" + strInNo + "', '0', '" + strInNo + "', '" + strIp + "' " +
        ////                    ",'" + strShape + "', '" + strMColor + "', " + strMQuality + ", " + strMCut + ", " + strMPolish + ", " + strMSym + ", " + strMFlor + " " +
        ////                    "," + strLabShape + ", " + strColor + ", " + strQuality + ", " + strCut + ", " + strPolish + ", " + strSym + ", " + strFlor + " " +
        ////                    ", " + strWhiteTable + ", " + strWhiteSide + ", " + strBlackTable + ", " + strBlackSide + ", " + strOpenTable + ", " + strOpenSide + ", " + strShade + " " +
        ////                    ",'1', '" + strOrdTranId + "',  '20', '" + strLocation + "', '1', " + strStockDate + ", '" + strTime + "', 'Sin', 'S', " + strRoughType + ", " + strVerId + ", " + strOpenPav + ", " + strOpenGrid +
        ////                    ", " + strMilky + ", " + strTsp + ", " + strSsp + ", " + strNt + ", " + strHa + ", " + strPFace + ", " + strCulet + ", '', " + strLuster + " " +
        ////                    "," + strCrnAg + ", " + strPavAg + ", " + strLan + ", " + strLrHalf + ", " + strGirdlePer + ", " + strCrnHt + ", " + strPavHt + " " +
        ////                    "," + strCertiNo + ", '" + strDiameter + "', " + strLength + ", " + strWidth + ", " + strHeight + ", " + strDepth + ", " + strTable + ", " + strEye + " " +
        ////                    ",'0', '" + strIsMovie + "', '" + strDate + "', '" + strTime + "', " + strShade + ", " + strWhiteTable1 + ", " + strWhiteSide1 + " " +
        ////                    ", '" + strRap + "', '" + strDisc + "',  '" + strRate + "', '" + strAmount + "' " +
        ////                    ",'" + dRap + "', '" + strMDisc + "',  '" + strMRate + "', '" + strMAmount + "' " +
        ////                    ", " + strLabDate + ", '" + strTime + "', '" + strDate + "', '" + strTime + "', '" + GlobalPara.CurrentUserId + "', '" + GlobalPara.CurrentUserId + "' " +
        ////                    ", " + strLabType + ", '0', '0',  '0', '0', '1', '0',  '0', '0', '', '' " +
        ////                    ", " + strKeyToSym + ", " + strIncription + ", '" + strLabRemarks + "', '" + tblData.Rows[i][3].ToString() + "' " +
        ////                    ",'" + dRap + "', '" + strMDisc + "',  '" + strMRate + "', '" + strMAmount + "' " +
        ////                    ",'0', '0',  '0', '0' " +
        ////                    ",'" + strRemark + "', " + strGirdleDesc + ", null, " + strLabType + ", '" + strColor + "', '" + strQuality + "' " +
        ////                    ", '" + strPurTypeName + "','" + strInNo + "','" + General.ReplcaeQuotString(strLabShapeName) + "','" + strLabNo + "','" + strCFace + "','" + strMfgRemarks + "' " +
        ////                    "," + strImgLink + "," + strVideoLink + "," + strReportDate + "," + strLabRevisedDate + "," + strLabName + "," + strLabSite + ",'" + strScsNO + "','" + strRfidNo + "','" + strCustId + "')";

        ////                queries.Add(query1);
        ////            }


        ////            if (strPurType == "58")
        ////            {

        ////                query1 = " INSERT INTO public.sale_history(roughid, packetid, stoneid, status, status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, janname, jandate, lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, mfg_remarks, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno, memono, export_partyid, sale_janno, statusname,  invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,iss_ip,certi_date,memo_name,lab_shapename,scsno,rfid,custid) " +
        ////                         " select roughid, packetid, stoneid,'53', status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, sale_janname, '" + strDate + "', lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, 1, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, remark, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno,  memono, export_partyid, sale_janno,'New_Arrival', invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,'" + GlobalPara.RequestIp() + "',certi_date,memo_name,lab_shapename,scsno,rfid,custid from sale_data where stoneid='" + strStoneId + "' ";

        ////                queries.Add(query1);

        ////                query1 = " INSERT INTO public.sale_history(roughid, packetid, stoneid, status, status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, janname, jandate, lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, mfg_remarks, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno, memono, export_partyid, sale_janno, statusname,  invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,iss_ip,certi_date,memo_name,lab_shapename,scsno,rfid,custid) " +
        ////                         " select roughid, packetid, stoneid,'56', status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, gcolorid, gqualid, gcutid, gpolid, gsymid, gflorid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, sale_janname, '" + strDate + "', lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, remark, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno,  memono, export_partyid, sale_janno,'PRFS', invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,'" + GlobalPara.RequestIp() + "',certi_date,memo_name,lab_shapename,scsno,rfid,custid from sale_data where stoneid='" + strStoneId + "' ";

        ////                queries.Add(query1);

        ////                query1 = " INSERT INTO public.sale_history(roughid, packetid, stoneid, status, status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, janname, jandate, lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, mfg_remarks, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno, memono, export_partyid, sale_janno, statusname,  invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,iss_ip,certi_date,memo_name,lab_shapename,scsno,rfid,custid) " +
        ////                         " select roughid, packetid, stoneid,'57', status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, gcolorid, gqualid, gcutid, gpolid, gsymid, gflorid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, sale_janname, '" + strDate + "', lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, remark, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno,  memono, export_partyid, sale_janno,'RFS', invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,'" + GlobalPara.RequestIp() + "',certi_date,memo_name,lab_shapename,scsno,rfid,custid from sale_data where stoneid='" + strStoneId + "' ";

        ////                queries.Add(query1);

        ////                query1 = " INSERT INTO public.sale_history(roughid, packetid, stoneid, status, status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, janname, jandate, lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, mfg_remarks, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno, memono, export_partyid, sale_janno, statusname,  invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,iss_ip,certi_date,memo_name,lab_shapename,scsno,rfid,custid) " +
        ////                         " select roughid, packetid, stoneid, '" + strPurType + "', status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, gcolorid, gqualid, gcutid, gpolid, gsymid, gflorid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, sale_janname, '" + strDate + "', lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, remark, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno,  memono, export_partyid, sale_janno, statusname, invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,'" + GlobalPara.RequestIp() + "',certi_date,memo_name,lab_shapename,scsno,rfid,custid from sale_data where stoneid='" + strStoneId + "' ";

        ////                queries.Add(query1);

        ////            }
        ////            else
        ////            {
        ////                query1 = " INSERT INTO public.sale_history(roughid, packetid, stoneid, status, status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, janname, jandate, lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, mfg_remarks, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno, memono, export_partyid, sale_janno, statusname,  invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,iss_ip,certi_date,memo_name,lab_shapename,scsno,rfid,custid) " +
        ////                        " select roughid, packetid, stoneid, '" + strPurType + "', status_flgid, rec_pcs, rec_weight, addid, adddate, addtime, partyid, shapeid, colorid, qualid, cutid, polid, symid, florid, gshapeid, whitetableid, whitesideid, blacktableid, blacksideid, opentableid, opensideid, shadeid, orderid, sale_empid, branchid, roughtypeid, openpavid, opengridid, milkyid, tsp, ssp, nat, ha, cface, paface, culet, luster, cro, pav, sta, lhl, gir, gmi, gmx, cro_per, pav_per, certino, certi_type, labid, avg_dia, max_dia, min_dia, height, d, t, is_eye, whitetableid1, whitesideid1, orgrap, perdis, labrate, rate, amount, labamt, gorgrap, gperdis, grate, gamount, janno, sale_janname, '" + strDate + "', lab_locid, lab_iss_date, lab_iss_time, lab_rec_date, lab_rec_time, lab_iss_id, lab_rec_id, lab_janno, lab_typeid, lab_add_per, lab_service, lab_recno, pricerid, porgrap, pperdis, prate, pamount, price_authid, price_date, price_time, auth_rate, auth_perdis, auth_amount, auth_date, auth_time, labno, lab_priority, corgrap, cperdis, crate, camount, offer_rap, offer_dis, offer_rate, offer_amount, sale_rap, sale_dis, sale_rate, sale_amount, keytosym, insc, remark, lab_iss_remarks, lab_rec_remarks, pricer_remarks, packet_entry_remarks, brokerid, controlno,  dorgrap, dperdis, drate, damount, gir_desc, last_offer, diff_per, diff_amount, isfancy, fcolorid, fintensityid, fovertoneid, old_dis, old_rate, old_amount,  conno,  memono, export_partyid, sale_janno, statusname, invoiceno, brokerage, sale_due_days, sale_due_date, sale_remarks, sale_type, bill_type, shipping_countryid, purno,'" + GlobalPara.RequestIp() + "',certi_date,memo_name,lab_shapename,scsno,rfid,custid from sale_data where stoneid='" + strStoneId + "' ";

        ////                queries.Add(query1);
        ////            }

        ////            iSrNo++;
        ////            iStoneSeqno++;


        ////            x++;
        ////        }
        ////        iStoneSeqno = iStoneSeqno - 1;
        ////        query1 = "update m_rough set pcs= coalesce(pcs,0) + '" + iPcs + "',weight = coalesce(weight,0) + '" + dTotWeight + "',stone_seqno=" + iStoneSeqno + " where tranid='" + strRoughId + "' ";
        ////        queries.Add(query1);

        ////        string[] queriesArr = queries.ToArray();
        ////        General.RunMultipleQueries(queriesArr);
        ////        ret = strInNo;

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        x++;
        ////        ret = "Error";
        ////        errMsg = ex.ToString();
        ////    }

        ////    return ret;
        ////}

        ////public String SaveImportRfidData(DataTable tblData, ref string errMsg)
        ////{
        ////    String ret = "";
        ////    int x = 0;
        ////    try
        ////    {

        ////        List<string> queries = new List<string>();
        ////        string query1 = "";
        ////        string strDate = GlobalPara.LoginDate();
        ////        string strTime = GlobalPara.Logintime();
        ////        string strInvalidStoneId = "";
        ////        int iStoneId = 0;
        ////        DataTable tbl = new DataTable();
        ////        for (int i = 0; i <= tblData.Rows.Count - 1; i++)
        ////        {
        ////            tbl = General.ReturnTable("select count(1) as cnt from m_packet where rfid='" + tblData.Rows[i][1].ToString().Trim() + "' ");
        ////            if (Convert.ToInt32(tbl.Rows[0]["cnt"]) == 0)
        ////            {
        ////                query1 = "update m_packet set rfid= '" + tblData.Rows[i][1].ToString().Trim() + "' where stoneid='" + tblData.Rows[i][0].ToString().Trim() + "' ";
        ////                queries.Add(query1);

        ////                query1 = "update sale_data set rfid= '" + tblData.Rows[i][1].ToString().Trim() + "' where stoneid='" + tblData.Rows[i][0].ToString().Trim() + "' ";
        ////                queries.Add(query1);

        ////                query1 = "update sale_history set rfid= '" + tblData.Rows[i][1].ToString().Trim() + "' where tranid=(select max(tranid) from sale_history where stoneid='" + tblData.Rows[i][0].ToString().Trim() + "' ) ";
        ////                queries.Add(query1);

        ////                iStoneId++;
        ////            }
        ////            else
        ////            {
        ////                strInvalidStoneId += tblData.Rows[i][0].ToString().Trim() + ",";
        ////            }
        ////            x++;
        ////        }

        ////        string[] queriesArr = queries.ToArray();
        ////        General.RunMultipleQueries(queriesArr);
        ////        ret = "";
        ////        errMsg = iStoneId + " Stones RFID Updated \n ";
        ////        if (strInvalidStoneId.Length > 1)
        ////        {
        ////            errMsg += " Invalid StoneId " + strInvalidStoneId;
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        x++;
        ////        ret = "Error";
        ////        errMsg = ex.ToString();
        ////    }

        ////    return ret;
        ////}
    }
}
