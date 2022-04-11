﻿using HomesForAll.DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HomesForAll.Utils.JWT;
using HomesForAll.DAL.Models.Authentication;
using HomesForAll.Utils.Mail;
using Serilog;

namespace HomesForAll.Services.AuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger _logger;

        public AuthenticationService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ILogger logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        
        public async Task<ResponseBase<EmptyResponseModel>> Register(RegistrationModel model)
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

                MailManager.SendRegistrationMail(user.Id, user.Email, user.Name);

                return new ResponseBase<EmptyResponseModel>()
                {
                    Success = true,
                    Message = "User created succesfully, waiting for email confirmation"
                };
            }
            catch (Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>()
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
                {
                    _logger.Information("Invalid login provided: {FullName}, model : {@Model}", typeof(AuthenticationService).FullName, model);
                    throw new Exception("Invalid login");
                    
                }
                    

                if (!user.EmailConfirmed)
                    throw new Exception("Email is not confirmed");

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.Id.ToString())
                };

                //Iterable for scalable design
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var refreshToken = TokenManager.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);

                await _userManager.UpdateAsync(user);

                var token = TokenManager.CreateToken(authClaims);

                

                return new ResponseBase<AuthenticationResponseModel>
                {
                    Success = true,
                    Message = "Logged in succesfully",
                    Body = new AuthenticationResponseModel
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshToken
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
        public async Task<ResponseBase<AuthenticationResponseModel>> RefreshToken(string authToken, string refreshToken)
        {
            try
            {
                if (authToken == null || refreshToken == null)
                    throw new Exception("Invalid authorization or refresh token");

                var principal = TokenManager.GetPrincipalFromExpiredToken(authToken);
                if (principal == null || principal.Identity == null)
                    throw new Exception("Invalid token");

                var user = await _userManager.FindByNameAsync(principal.Identity.Name);
                if (user == null)
                    throw new Exception("User doesn't exist");

                if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryDate < DateTime.Now)
                    throw new Exception("Invalid refresh token");

                var newRefreshToken = TokenManager.GenerateRefreshToken();
                var newAuthToken = TokenManager.CreateToken(principal.Claims.ToList());

                user.RefreshToken = newRefreshToken;
                await _userManager.UpdateAsync(user);

                return new ResponseBase<AuthenticationResponseModel>
                {
                    Success = true,
                    Message="Succesfully refreshed token",
                    Body = new AuthenticationResponseModel
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(newAuthToken),
                        RefreshToken = newRefreshToken
                    }
                };


            }catch(Exception ex)
            {
                return new ResponseBase<AuthenticationResponseModel>
                {
                    Success= false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ResponseBase<EmptyResponseModel>> VerifyEmail(string userId)
        {
            try
            {
                
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new Exception("Invalid user id");

                if (user.EmailConfirmed)
                    throw new Exception("Email has already been confirmed");

                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                return new ResponseBase<EmptyResponseModel>
                {
                    Success = true,
                    Message = "Email succesfully confirmed"
                };

            }catch(Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success=false,
                    Message=ex.Message
                };
            }
        }
    }
}
