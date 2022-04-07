using HomesForAll.DAL.Entities;
//using HomesForAll.DAL.Models.Property;
using HomesForAll.DAL.UserRoles;
using HomesForAll.Services.PropertyServices;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.PropertyModels;
using HomesForAll.Utils.ServerResponse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomesForAll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet("getAll")]
        [Authorize(Roles = $"{Roles.Tenant},{Roles.Landlord}")]
        public async Task<ActionResult<ResponseBase<List<Property>>>> GetAllProperties()
        {
            var result = await _propertyService.GetAllProperties();

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getById/{propertyId}")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<List<Property>>>> GetPropertyById([FromRoute] string propertyId)
        {
            var result = await _propertyService.GetProperty(propertyId);

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

    }
}
