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

        [HttpGet("getPropertyRequests")]
        [Authorize(Roles = Roles.Landlord)]
        public async Task<ActionResult<ResponseBase<List<GetRequestResponseModel>>>> GetRequests([FromHeader] string authorization)
        {
            var result = await _landlordService.GetRequests(authorization);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("registerProperty")]
        [Authorize(Roles = Roles.Landlord)]
        public async Task<ActionResult<ResponseBase<RegisterPropertyResponseModel>>> RegisterProperty([FromBody] RegisterPropertyModel model, [FromHeader] string authorization)
        {
            var result = await _landlordService.RegisterProperty(model, authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("getCurrent")]
        [Authorize(Roles = Roles.Landlord)]
        public async Task<ActionResult<ResponseBase<GetLandlordResponseModel>>> GetLandlord([FromHeader] string authorization)
        {
            var result = await _landlordService.GetLandlord(authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("updateCurrent")]
        [Authorize(Roles = Roles.Landlord)]
        public async Task<ActionResult<ResponseBase<GetLandlordResponseModel>>> UpdateLandlord([FromBody] UpdateLandlordModel model, [FromHeader] string authorization)
        {
            var result = await _landlordService.UpdateLandlord(model, authorization);

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("acceptRequest/{requestId}")]
        [Authorize(Roles= Roles.Landlord)]
        public async Task<ActionResult<ResponseBase<EmptyResponseModel>>> AcceptRequest([FromRoute] string requestId)
        {
            var result = await _landlordService.AcceptRequest(requestId);

            if(result.Success) return Ok(result);
            return BadRequest(result);
        }

    }
}
