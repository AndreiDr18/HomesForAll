using HomesForAll.Services.LandlordServices;
using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Landlord;
using HomesForAll.DAL.UserRoles;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomesForAll.Utils.ServerResponse.Models.LandlordModels;
using HomesForAll.Utils.ServerResponse.Models.TenantModels;

namespace HomesForAll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandlordController : ControllerBase
    {

        private readonly ILandlordService _landlordService;

        public LandlordController(ILandlordService landlordService)
        {
            _landlordService = landlordService;
        }

        /// <summary>
        ///     Get all tenancy requests sent to every property owned
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet("getPropertyRequests")]
        //[Authorize(Roles = Roles.Landlord)]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<List<GetTenantRequestResponseModel>>>> GetRequests([FromHeader] string authorization)
        {
            var result = await _landlordService.GetRequests(authorization);

            return Ok(result);
            
        }

        /// <summary>
        ///     Register property to be up for tenancy requests
        /// </summary>
        /// <param name="model"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpPost("registerProperty")]
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> RegisterProperty([FromBody] RegisterPropertyModel model, [FromHeader] string authorization)
        {
            var result = await _landlordService.RegisterProperty(model, authorization);

            return Ok(result);
        }

        /// <summary>
        ///     Delete a property
        /// </summary>
        /// <param name="propertyId"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpDelete("deleteProperty/{propertyId}")]
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> DeleteProperty([FromRoute] string propertyId, [FromHeader] string authorization)
        {
            var result = await _landlordService.DeleteProperty(propertyId, authorization);

            return Ok(result);
        }

        /// <summary>
        ///     Get owned properties
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet("getProperties")]
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<List<GetOwnedPropertyResponseModel>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<List<PropertyResponseModel>>>> DeleteProperty([FromHeader] string authorization)
        {
            var result = await _landlordService.GetProperties(authorization);

            return Ok(result);
        }

        /// <summary>
        ///     Get user information about current landlord
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet("getLandlord")]
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<GetLandlordResponseModel>>> GetLandlord([FromHeader] string authorization)
        {
            var result = await _landlordService.GetLandlord(authorization);

            return Ok(result);
        }

        /// <summary>
        ///     Update user information regarding current landlord
        /// </summary>
        /// <param name="model"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpPut("updateLandlord")]
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<GetLandlordResponseModel>>> UpdateLandlord([FromBody] UpdateLandlordModel model, [FromHeader] string authorization)
        {
            var result = await _landlordService.UpdateLandlord(model, authorization);

            return Ok(result);
        }

        /// <summary>
        ///     Accept tenancy request based on it's corresponding id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet("acceptRequest/{requestId}")]
        [Authorize(Roles= Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> AcceptRequest([FromRoute] string requestId, [FromHeader] string authorization)
        {
            var result = await _landlordService.AcceptRequest(requestId, authorization);

            return Ok(result);
        }

        /// <summary>
        ///     Revoke tenancy request based on its id
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpDelete("revokeRequest/{requestId}")]
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> RevokeRequest([FromRoute] string requestId, [FromHeader] string authorization)
        {
            var result = await _landlordService.RevokeRequest(requestId, authorization);

            return Ok(result);
        }

        /// <summary>
        ///     Revoke tenancy request based on its id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpDelete("evictTenant/{tenantId}")]
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> EvictTenant([FromRoute] string tenantId, [FromHeader] string authorization)
        {
            var result = await _landlordService.EvictTenant(authorization, tenantId);

            return Ok(result);
        }

    }
}
