﻿using HomesForAll.Services.LandlordServices;
using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Landlord;
using HomesForAll.DAL.UserRoles;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomesForAll.Utils.ServerResponse.Models.LandlordModels;

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
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<List<GetTenantRequestResponseModel>>>> GetRequests([FromHeader] string authorization)
        {
            var result = await _landlordService.GetRequests(authorization);
            if (result.Success) return Ok(result);
            return BadRequest(result);
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
        public async Task<ActionResult<ResponseBase<RegisterPropertyResponseModel>>> RegisterProperty([FromBody] RegisterPropertyModel model, [FromHeader] string authorization)
        {
            var result = await _landlordService.RegisterProperty(model, authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Get user information about current landlord
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet("getCurrent")]
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<GetLandlordResponseModel>>> GetLandlord([FromHeader] string authorization)
        {
            var result = await _landlordService.GetLandlord(authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Update user information regarding current landlord
        /// </summary>
        /// <param name="model"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet("updateCurrent")]
        [Authorize(Roles = Roles.Landlord)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<GetLandlordResponseModel>>> UpdateLandlord([FromBody] UpdateLandlordModel model, [FromHeader] string authorization)
        {
            var result = await _landlordService.UpdateLandlord(model, authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result);
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

            if(result.Success) return Ok(result);
            return BadRequest(result);
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

            if (result.Success) return Ok(result);
            return BadRequest(result);
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

            if(result.Success) return Ok(result);
            return BadRequest(result);
        }

    }
}
