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
        
        [HttpGet("getCurrentUserInfo")]
        [Authorize(Roles = Roles.Tenant)]
        public async Task<ActionResult<ResponseBase<GetByIdBodyModel>>> Get([FromHeader] string authorization)
        {
            
            var result = await _tenantService.GetTenantInfo(authorization);
            
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

            
    }
}
