using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomesForAll.DAL;
using HomesForAll.DAL.Models.Tenant;
using HomesForAll.DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;

namespace HomesForAll.Services.TenantServices
{
    public class TenantService : ITenantService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public TenantService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
        }
        
        public async Task<ResponseBase<TenantRegistrationBodyModel>> RegisterTenant(TenantRegisterModel model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Username);
                
                
                if (userExists != null)
                    throw new Exception("User already exists");


                User user = new User()
                {
                    Name = model.Name,
                    UserName = model.Username,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = model.Email
                };

                var res = await _userManager.CreateAsync(user, model.Password);
                _userManager.Dispose();

                if (!res.Succeeded) throw new Exception($"User could not be created: {res.Errors.ToArray().ToString()}");

                return new ResponseBase<TenantRegistrationBodyModel>()
                {
                    Success = true,
                    Message = "User created succesfully",
                    Body = new TenantRegistrationBodyModel
                    {
                        token = "Test",
                        Username = model.Username
                    }
            };
            }
            catch (Exception ex)
            {
                return new ResponseBase<TenantRegistrationBodyModel>()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
