using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomesForAll.DAL.Models.Tenant;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;
using HomesForAll.Services.TenantServices;

namespace HomesForAll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ITenantService _tenantService;

        public AuthController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }
        [HttpPost("tenant/register")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseBase<TenantRegistrationBodyModel>>> Register([FromBody]TenantRegisterModel model)
        {
            var result = await _tenantService.RegisterTenant(model);

            if(result.Success) return Ok(result);
            return BadRequest(result);


        }
    }
}
