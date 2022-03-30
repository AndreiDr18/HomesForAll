using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomesForAll.DAL.Models.Authentication;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;
using HomesForAll.Services.AuthenticationServices;

namespace HomesForAll.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseBase<AuthenticationBodyModel>>> Register([FromBody]RegistrationModel model)
        {
            var result = await _authenticationService.Register(model);

            if(result.Success) return Ok(result);
            return BadRequest(result);

        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseBase<AuthenticationBodyModel>>> Login([FromBody] LoginModel model)
        {
            var result = await _authenticationService.Login(model);

            if (result.Success) return Ok(result);
            return BadRequest(result);

        }
    }
}
