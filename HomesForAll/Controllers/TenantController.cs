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
        
        [HttpGet("getCurrent")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<GetTenantResponseModel>>> Get([FromHeader] string authorization)
        {
            
            var result = await _tenantService.GetTenantInfo(authorization);
            
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("updateCurrent")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> UpdateTenant([FromBody] TenantUpdateModel model, [FromHeader] string authorization)
        {
            var result = await _tenantService.UpdateTenant(model, authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result); 

        }
        [HttpPost("requestProperty")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> SendTenantRequest([FromBody] TenantRequestModel model, [FromHeader] string authorization)
        {
            var result = await _tenantService.SendTenantRequest(model, authorization);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getCurrentRequests")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<List<GetRequestResponseModel>>>> GetTenantRequests([FromHeader] string authorization)
        {
            var result = await _tenantService.GetTenantRequests(authorization);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("removeRequest/{reqId}")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> DeleteRequest([FromHeader] string authorization, [FromRoute] string reqId)
        {
            var result = await _tenantService.DeleteRequest(authorization, reqId);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
