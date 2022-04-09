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
using Microsoft.EntityFrameworkCore;

namespace HomesForAll.Services.TenantServices
{
    public class TenantService : ITenantService
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _dbContext;

        public TenantService(UserManager<User> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<ResponseBase<GetTenantResponseModel>> GetTenantInfo(string authToken)
        {
            try
            {
                var id = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

                var user = _dbContext.Users
                                     .Include(u => u.PropertyRequests)
                                     .ThenInclude(pr => pr.Property)
                                     .FirstOrDefault(u => u.Id == Guid.Parse(id));
                if (user == null)
                    throw new Exception("Couldn't fetch tenant info");

                List<TenantRequestResponseModel> PropertyRequests = new();

                if (user.PropertyRequests.Count != 0)
                    foreach (var propertyRequest in user.PropertyRequests)
                    {

                        PropertyRequests.Add(new TenantRequestResponseModel
                        {
                            RequestID = propertyRequest.Id,
                            NumberOfPeople = propertyRequest.NumberOfPeople,
                            Message = propertyRequest.Message,
                            Status = propertyRequest.Status.ToString(),
                            Property = new PropertyResponseModel
                            {
                                Name = propertyRequest.Property.Name,
                                AvailableSpaces = propertyRequest.Property.AvailableSpaces,
                                AddedAt = propertyRequest.Property.AddedAt,
                                Address = propertyRequest.Property.Address
                            }
                        });
                    }

                return new ResponseBase<GetTenantResponseModel>
                {
                    Success = true,
                    Message = "User information succesfully retrieved",
                    Body = new GetTenantResponseModel
                    {
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        DateOfBirth = user.BirthDate,
                        Username = user.UserName,
                        PropertyRequests = PropertyRequests
                    }
                };

            }
            catch (Exception ex)
            {
                return new ResponseBase<GetTenantResponseModel>
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
        public async Task<ResponseBase<EmptyResponseModel>> SendTenantRequest(TenantRequestModel model, string authToken)
        {
            try
            {
                var tenantId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                var tenant = await _userManager.FindByIdAsync(tenantId);
                var property = _dbContext.Properties.FirstOrDefault(p => p.Id == model.PropertyId);

                var requestExists = _dbContext.TenantRequests.Where(tr => tr.TenantID == Guid.Parse(tenantId) && tr.PropertyID == property.Id).Any();

                if (tenantId == null || tenant == null || property == null)
                    throw new Exception("Tenant or property couldn't be fetched");

                if (tenant.AcceptedAtPropertyID != null)
                    throw new Exception("Tenant has already been accepted to a property");

                if (requestExists)
                    throw new Exception("You can't register multiple requests to the same property");

                TenantRequest tenantRequest = new TenantRequest()
                {
                    Id = Guid.NewGuid(),
                    NumberOfPeople = model.NumberOfPeople,
                    Message = model.Message,
                    Status = Status.Pending,
                    Property = property,
                    PropertyID = model.PropertyId,
                    Tenant = tenant,
                    TenantID = Guid.Parse(tenantId)
                };
                await _dbContext.TenantRequests.AddAsync(tenantRequest);
/*                property.TenantRequests.Add(tenantRequest);
                tenant.PropertyRequests.Add(tenantRequest);*/

                var changes = _dbContext.SaveChanges();
                if (changes == 0)
                    throw new Exception("Request did not get registered");

                return new ResponseBase<EmptyResponseModel>
                {
                    Success = true,
                    Message = "Request got succesfully registered"
                };

            }
            catch (Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success=false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ResponseBase<List<GetRequestResponseModel>>> GetTenantRequests(string authToken)
        {
            try
            {
                var tenantId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                List<TenantRequest> tenantRequests = _dbContext.TenantRequests
                                                               .Include(x => x.Property)
                                                               .Where(tr => tr.TenantID == Guid.Parse(tenantId))
                                                               .ToList();

                if (tenantRequests.Count == 0)
                    return new ResponseBase<List<GetRequestResponseModel>>
                    {
                        Success = true,
                        Message = "Tenant has no registered requests"
                    };

                List<GetRequestResponseModel> tenantRequestsResponse = new List<GetRequestResponseModel>();

                foreach (var tenantRequest in tenantRequests)
                {
                    tenantRequestsResponse.Add(new GetRequestResponseModel
                    {
                        RequestID = tenantRequest.Id,
                        NumberOfPeople = tenantRequest.NumberOfPeople,
                        Message = tenantRequest.Message,
                        Status = tenantRequest.Status.ToString(),
                        Property = new PropertyResponseModel
                        {
                            Name = tenantRequest.Property.Name,
                            AvailableSpaces = tenantRequest.Property.AvailableSpaces,
                            AddedAt = tenantRequest.Property.AddedAt,
                            Address = tenantRequest.Property.Address
                        }

                    });
                }

                return new ResponseBase<List<GetRequestResponseModel>>
                {
                    Success = true,
                    Message = "Succesfully retrieved tenant requests",
                    Body = tenantRequestsResponse
                };

            }
            catch (Exception ex)
            {
                return new ResponseBase<List<GetRequestResponseModel>>
                {
                    Success=false,
                    Message=ex.Message
                };
            }
        }
        public async Task<ResponseBase<EmptyResponseModel>> DeleteRequest(string authToken, string reqId)
        {
            try
            {
                var tenantRequest = _dbContext.TenantRequests.Where(tr => tr.Id == Guid.Parse(reqId)).FirstOrDefault();
                var tenantId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

                if (tenantRequest == null)
                    throw new Exception("There is no request matching the given id");
                if (tenantRequest.Status == Status.Accepted)
                    throw new Exception("The request has already been accepted");
                if (tenantRequest.TenantID != Guid.Parse(tenantId))
                    throw new Exception("You can only delete requests that are your own");

                _dbContext.TenantRequests.Remove(tenantRequest);
                var changes = await _dbContext.SaveChangesAsync();

                if (changes == 0)
                    throw new Exception("Could not remove tenant request");

                return new ResponseBase<EmptyResponseModel>
                {
                    Success = true,
                    Message = "Succesfully removed tenant request"
                };


            }
            catch (Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success= false,
                    Message=ex.Message
                };
            }

        }
        //Get Accepted Property Details
    }
}
