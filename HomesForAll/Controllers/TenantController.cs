using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomesForAll.DAL.UserRoles;
using HomesForAll.DAL.Models.Tenant;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Services.TenantServices;
using HomesForAll.Utils.ServerResponse.Models.TenantModels;

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
        
        [HttpGet]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<GetByIdResponseModel>>> Get([FromHeader] string authorization)
        {
            
            var result = await _tenantService.GetTenantInfo(authorization);
            
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> UpdateTenant([FromBody] TenantUpdateModel model, [FromHeader] string authorization)
        {
            var result = await _tenantService.UpdateTenant(model, authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result); 

        }
    }
}
