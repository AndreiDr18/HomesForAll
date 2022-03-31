using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Property;
using HomesForAll.DAL.UserRoles;
using HomesForAll.Services.PropertyServices;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.PropertyModels;
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

        [HttpGet]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<List<Property>>>> GetAllProperties()
        {
            var result = await _propertyService.GetAllProperties();

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
        [HttpPost]
        [Authorize(Roles = Roles.Landlord)]
        public async Task<ActionResult<ResponseBase<RegisterPropertyResponseModel>>> RegisterProperty([FromBody] RegisterPropertyModel model, [FromHeader] string authorization)
        {
            var result = await _propertyService.RegisterProperty(model, authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
