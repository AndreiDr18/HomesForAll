using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomesForAll.DAL.Models.Authentication;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;
using HomesForAll.Services.AuthenticationServices;
using Swashbuckle.Swagger.Annotations;

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

        ///<summary>
        ///     Register a user
        ///</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>),StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> Register([FromBody]RegistrationModel model)
        {
            var result = await _authenticationService.Register(model);
            
            return Ok(result);

        }

        /// <summary>
        ///     Exchange username and password for bearer authorization token
        /// </summary>
        /// <param name="model"></param>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<AuthenticationResponseModel>>> Login([FromBody] LoginModel model)
        {
            var result = await _authenticationService.Login(model);

            return Ok(result);

        }
        /// <summary>
        ///     Exchange an expired authorization token alongside its corresponding refresh token for new ones
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpGet("refreshToken")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<AuthenticationResponseModel>>> RefreshToken([FromHeader] string authorization, [FromHeader] string refreshToken)
        {
            var result = await _authenticationService.RefreshToken(authorization, refreshToken);

            return Ok(result);
        }

        /// <summary>
        ///     Email verification for newly registered users
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("verifyEmail/{userId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseBase<EmptyResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> VerifyEmail([FromRoute] string userId)
        {
            var result = await _authenticationService.VerifyEmail(userId);

            return Ok(result);
        }
    }
}
