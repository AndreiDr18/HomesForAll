using HomesForAll.DAL.Entities;
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

        /// <summary>
        ///     Get every property registered
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAll")]
        [Authorize(Roles = $"{Roles.Tenant},{Roles.Landlord}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<List<GetPropertyResponseModel>>>> GetAllProperties()
        {
            var result = await _propertyService.GetAllProperties();

            return Ok(result);
        }

        /// <summary>
        ///     Get a single property based on its id
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [HttpGet("getById/{propertyId}")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<List<GetPropertyResponseModel>>>> GetPropertyById([FromRoute] string propertyId)
        {
            var result = await _propertyService.GetProperty(propertyId);

            return Ok(result);
        }

    }
}
