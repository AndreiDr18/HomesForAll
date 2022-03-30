﻿using HomesForAll.DAL;
using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Tenant;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.TenantModels;
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
        public async Task<ResponseBase<GetByIdBodyModel>> GetTenantInfo(string authToken)
        {
            try
            {
                string jwt;

                if (AuthenticationHeaderValue.TryParse(authToken, out var header))
                    jwt = header.Parameter;
                else throw new Exception("Couldn't parse authorization token from header");


                var tokenJSON = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

                var id = tokenJSON.Claims.FirstOrDefault(cl => cl.Type == "UserId").Value;

                var user = await _userManager.FindByIdAsync(id);

                return new ResponseBase<GetByIdBodyModel>
                {
                    Success = true,
                    Message = "User information succesfully retrieved",
                    Body = new GetByIdBodyModel
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
                return new ResponseBase<GetByIdBodyModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            
        }
        public async Task<ResponseBase<EmptyBodyModel>> UpdateTenant(TenantUpdateModel model, string authToken)
        {
            try
            {
                string jwt;

                if (AuthenticationHeaderValue.TryParse(authToken, out var header))
                    jwt = header.Parameter;
                else throw new Exception("Couldn't parse authorization token from header");


                var tokenJSON = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

                var id = tokenJSON.Claims.FirstOrDefault(cl => cl.Type == "UserId").Value;

                var user = await _userManager.FindByIdAsync(id);

                user.Name = model.Name;
                user.PhoneNumber = model.PhoneNumber;
                user.BirthDate = model.BirthDate;

                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                    throw new Exception("User could not be updated");

                return new ResponseBase<EmptyBodyModel>
                {
                    Success = true,
                    Message = "User data succesfully updated"
                };
            }catch (Exception ex)
            {
                return new ResponseBase<EmptyBodyModel>
                {
                    Success = false,
                    Message= ex.Message
                };
            }
            
        }
    }
}
