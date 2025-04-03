using System;
using System.Threading.Tasks;
using Business.Repository.IRepository;
using DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiamondPropertyController : ControllerBase
    {
        private readonly IDiamondPropertyRepository _diamondProperty;
        private readonly IWebHostEnvironment _env;

        public DiamondPropertyController(IDiamondPropertyRepository diamondProperty, IWebHostEnvironment env)
        {
            _diamondProperty = diamondProperty;
            _env = env;
        }

        #region GET Diamond Properties

        [HttpGet("diamond-property-list")]
        public async Task<ActionResult> GetDiamondProperies()
        {
            var result = await _diamondProperty.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("diamond-property/diamond-property-id/{id}")]
        public async Task<ActionResult> GetDiamondPropery(int id)
        {
            var result = await _diamondProperty.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("diamond-property/get-metal-list")]
        public async Task<ActionResult> GetDiamondMetal()
        {
            var result = await _diamondProperty.GetMetalListAsync();
            return Ok(result);
        }

        [HttpGet("diamond-property/get-carat-list")]
        public async Task<ActionResult> GetDiamondCarat()
        {
            var result = await _diamondProperty.GetCaratListAsync();
            return Ok(result);
        }

        [HttpGet("diamond-property/get-clarity-list")]
        public async Task<ActionResult> GetClarityListAsync()
        {
            var result = await _diamondProperty.GetClarityListAsync();
            return Ok(result);
        }

        [HttpGet("diamond-property/get-shape-list")]
        public async Task<ActionResult> GetShapeListAsync()
        {
            var result = await _diamondProperty.GetShapeListAsync();
            return Ok(result);
        }

        #endregion

        [HttpPost("add-diamond-property")]
        public async Task<ActionResult> AddDiamondProperty([FromForm] DiamondProperty diamondProperty)
        {
            try
            {
                var result= await _diamondProperty.AddAsync(diamondProperty);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpPost("update-diamond-property")]
        public async Task<ActionResult> UpdateDiamondProperty(DiamondProperty diamondProperty)
        {
            try
            {
                var result = await _diamondProperty.UpdateAsync(diamondProperty);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpPost("delete-diamond-property")]
        public async Task<ActionResult> DeleteDiamondProperty(int id)
        {
            try
            {
                var result = await _diamondProperty.DeleteAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }    
        }

        [HttpPost("Get-diamond-property/propertyValue/{propertyValue}/diamondProperty/{diamondProperty}")]
        public async Task<ActionResult> UpsertDiamondProperty(string propertyValue, string diamondProperty)
        {
            try
            {
                var result = await _diamondProperty.GetDiamondPropertyId(propertyValue, diamondProperty);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
