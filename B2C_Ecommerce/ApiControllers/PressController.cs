using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PressController : ControllerBase
    {
        [HttpGet("TestMethod")]
        public async Task<DataTable> MyFunction()
        {
            var dt = new DataTable();
            dt.Columns.Add("SrNo");
            dt.Columns.Add("Name");

            DataRow dr = dt.NewRow();
            dr["SrNo"] = "1";
            dr["Name"] = "XYZ";
            dt.Rows.Add(dr);

            return dt;
        }
    }
}
