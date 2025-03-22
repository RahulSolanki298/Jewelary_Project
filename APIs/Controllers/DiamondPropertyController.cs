using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondPropertyController : ControllerBase
    {
        

        public DiamondPropertyController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> GetDiamondProperies()
        {
            return Ok();
        }

    }
}
