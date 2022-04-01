using HomesForAll.DAL;
using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Tenant;
using HomesForAll.Utils.JWT;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.TenantModels;
using HomesForAll.Utils.ServerResponse.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Services.TenantServices
{
    public class TenantService : ITenantService
    {
        private readonly UserManager<User> _userManager;

        public TenantService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ResponseBase<GetByIdResponseModel>> GetTenantInfo(string authToken)
        {
            try
            {
                var id = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

                var user = await _userManager.FindByIdAsync(id);

                return new ResponseBase<GetByIdResponseModel>
                {
                    Success = true,
                    Message = "User information succesfully retrieved",
                    Body = new GetByIdResponseModel
                    {
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        DateOfBirth = user.BirthDate,
                        Username = user.UserName
                    }
                };

            }
            catch (Exception ex)
            {
                return new ResponseBase<GetByIdResponseModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            
        }
        public async Task<ResponseBase<EmptyResponseModel>> UpdateTenant(TenantUpdateModel model, string authToken)
        {
            try
            {

                var id = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

                var user = await _userManager.FindByIdAsync(id);

                user.Name = model.Name;
                user.PhoneNumber = model.PhoneNumber;
                user.BirthDate = model.BirthDate;

                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                    throw new Exception("User could not be updated");

                return new ResponseBase<EmptyResponseModel>
                {
                    Success = true,
                    Message = "User data succesfully updated"
                };
            }catch (Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success = false,
                    Message= ex.Message
                };
            }
            
        }
    }
}
