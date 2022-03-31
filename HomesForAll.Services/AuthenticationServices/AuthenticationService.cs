using HomesForAll.DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HomesForAll.Utils.JWT;
using HomesForAll.DAL.Models.Authentication;

namespace HomesForAll.Services.AuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
        }
        
        public async Task<ResponseBase<AuthenticationResponseModel>> Register(RegistrationModel model)
        {
            try
            {
                var userEntry = await _userManager.FindByNameAsync(model.Username);

                if (userEntry != null)
                    throw new Exception("User already exists");
                var emailEntry = _userManager.FindByEmailAsync(model.Email).Result;

                if (emailEntry != null)
                    throw new Exception("Another account is registered with the same email");

                var inputRoleExists = await _roleManager.RoleExistsAsync(model.Role);

                if (!inputRoleExists)
                    throw new Exception($"{model.Role} role doesn't exist");


                User user = new User()
                {
                    Name = model.Name,
                    UserName = model.Username,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    BirthDate = model.BirthDate,
                    JoinedAtDate = DateTime.UtcNow

                };

                var res = await _userManager.CreateAsync(user, model.Password);

                if (!res.Succeeded) 
                    throw new Exception($"User could not be created: {res.Errors.ToArray().ToString()}");

                await _userManager.AddToRoleAsync(user, model.Role);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, model.Role),
                    new Claim("UserId", user.Id)
                };
                
                var token = TokenManager.CreateToken(authClaims, _configuration);

                return new ResponseBase<AuthenticationResponseModel>()
                {
                    Success = true,
                    Message = "User created succesfully",
                    Body = new AuthenticationResponseModel
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                        
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseBase<AuthenticationResponseModel>()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ResponseBase<AuthenticationResponseModel>> Login(LoginModel model)
        {
            
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                    throw new Exception("Invalid login");

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.Id)
                };

                //Iterable for scalable design
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = TokenManager.CreateToken(authClaims, _configuration);

                return new ResponseBase<AuthenticationResponseModel>
                {
                    Success = true,
                    Message = "Logged in succesfully",
                    Body = new AuthenticationResponseModel
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token)
                    }
                };
                

            }
            catch(Exception ex)
            {
                return new ResponseBase<AuthenticationResponseModel>
                {
                    Success= false,
                    Message= ex.Message
                };
            }
        }
    }
}
