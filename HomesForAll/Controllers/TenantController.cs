using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomesForAll.DAL.UserRoles;
using HomesForAll.DAL.Models.Tenant;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Services.TenantServices;
using HomesForAll.Utils.ServerResponse.Models.TenantModels;
using HomesForAll.Utils.ServerResponse.Models;

namespace HomesForAll.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private ITenantService _tenantService;

        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }
        
        /// <summary>
        ///     Get tenant information
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet("getCurrent")]
        [Authorize(Roles = Roles.Tenant)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<GetTenantResponseModel>>> GetTenantInfo([FromHeader] string authorization)
        {
            
            var result = await _tenantService.GetTenantInfo(authorization);
            
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Update tenant information
        /// </summary>
        /// <param name="model"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpPut("updateCurrent")]
        [Authorize(Roles = Roles.Tenant)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> UpdateTenant([FromBody] TenantUpdateModel model, [FromHeader] string authorization)
        {
            var result = await _tenantService.UpdateTenant(model, authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result); 

        }

        /// <summary>
        ///     Send a tenancy request
        /// </summary>
        /// <param name="model"></param>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpPost("requestProperty")]
        [Authorize(Roles = Roles.Tenant)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> SendTenantRequest([FromBody] TenantRequestModel model, [FromHeader] string authorization)
        {
            var result = await _tenantService.SendTenantRequest(model, authorization);
            if (result.Success) return Created("https://localhost:7165",result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Get tenancy requests sent from a single tenant
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet("getCurrentRequests")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<List<GetRequestResponseModel>>>> GetTenantRequests([FromHeader] string authorization)
        {
            var result = await _tenantService.GetTenantRequests(authorization);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Delete tenancy request
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="reqId"></param>
        /// <returns></returns>
        [HttpDelete("removeRequest/{reqId}")]
        [Authorize(Roles = Roles.Tenant)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> DeleteRequest([FromHeader] string authorization, [FromRoute] string reqId)
        {
            var result = await _tenantService.DeleteRequest(authorization, reqId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Get Landlord information from the property to which tenant's been accepted
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        [HttpGet("getLandlordContactDetails")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<GetAcceptedAtLandlordInfo>>> GetLandlordContactDetails([FromHeader] string authorization)
        {
            var result = await _tenantService.GetLandlordInfo(authorization);

            if(result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
