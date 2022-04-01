using HomesForAll.DAL;
using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Property;
using HomesForAll.Utils.JWT;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.PropertyModels;
using HomesForAll.Utils.ServerResponse.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Services.PropertyServices
{
    public class PropertyService : IPropertyService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public PropertyService(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        //Tenant Access
        public async Task<ResponseBase<List<GetAllPropertiesResponseModel>>> GetAllProperties()
        {
            try
            {
                var properties = _dbContext.Properties.ToList();
                if (properties.Count == 0)
                    throw new Exception("There are no registered properties");

                List<GetAllPropertiesResponseModel> Body = new List<GetAllPropertiesResponseModel>();

                foreach(var property in properties)
                {
                    Body.Add(new GetAllPropertiesResponseModel
                    {
                        Id = property.Id,
                        Name = property.Name,
                        Address = property.Address,
                        AvailableSpaces = property.AvailableSpaces,
                        AddedAt = property.AddedAt
                    });
                }
                return new ResponseBase<List<GetAllPropertiesResponseModel>>
                {
                    Success = true,
                    Message = "Succesfully retrieved all properties",
                    Body = Body
                };
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<GetAllPropertiesResponseModel>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ResponseBase<EmptyResponseModel>> RequestProperty(RequestPropertyModel model, string authToken)
        {
            try
            {
                var tenantId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                var tenant = await _userManager.FindByIdAsync(tenantId);
                var property = _dbContext.Properties.FirstOrDefault(p => p.Id == model.PropertyId);

                if (tenantId == null || tenant == null || property == null)
                    throw new Exception("Tenant or property couldn't be fetched");

                var requestExists = _dbContext.TenantRequests.Where(tr => tr.TenantID == tenantId && tr.PropertyID == property.Id).Any();

                if (requestExists)
                    throw new Exception("You can't register multiple requests to the same property");

                TenantRequest tenantRequest = new TenantRequest()
                {
                    Id = Guid.NewGuid().ToString(),
                    NumberOfPeople = model.NumberOfPeople,
                    Message = model.Message,
                    Property = property,
                    PropertyID = model.PropertyId,
                    Tenant = tenant,
                    TenantID = tenantId
                };
                await _dbContext.TenantRequests.AddAsync(tenantRequest);
                property.TenantRequests.Add(tenantRequest);
                tenant.PropertyRequests.Add(tenantRequest);

                var result = _dbContext.SaveChanges();
                if (result == 0)
                    throw new Exception("Request did not get registered");

                return new ResponseBase<EmptyResponseModel>
                {
                    Success = true,
                    Message = "Request got succesfully registered"
                };

            }catch(Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success=false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ResponseBase<List<GetTenantRequestsResponseModel>>> GetTenantRequests(string authToken)
        {
            try
            {
                var tenantId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                List<TenantRequest> tenantRequests = _dbContext.TenantRequests.Where(tr => tr.TenantID == tenantId).ToList();

                if (tenantRequests.Count == 0)
                    return new ResponseBase<List<GetTenantRequestsResponseModel>>
                    {
                        Success = true,
                        Message = "Tenant has no registered requests"
                    };

                List<GetTenantRequestsResponseModel> tenantRequestsResponse = new List<GetTenantRequestsResponseModel>();

                foreach(var tenantRequest in tenantRequests)
                {
                    tenantRequestsResponse.Add(new GetTenantRequestsResponseModel
                    {
                        RequestID = tenantRequest.Id,
                        NumberOfPeople = tenantRequest.NumberOfPeople,
                        Message = tenantRequest.Message,
                        TenantID = tenantRequest.TenantID,
                        PropertyID = tenantRequest.PropertyID

                    });
                }

                return new ResponseBase<List<GetTenantRequestsResponseModel>>
                {
                    Success = true,
                    Message = "Succesfully retrieved tenant requests",
                    Body = tenantRequestsResponse
                };

            }catch(Exception ex)
            {
                return new ResponseBase<List<GetTenantRequestsResponseModel>>
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
                var tenantRequest = _dbContext.TenantRequests.Where(tr => tr.Id == reqId).FirstOrDefault();
                if (tenantRequest == null)
                    throw new Exception("There is no request matching the given id");
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
            catch(Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success= false,
                    Message=ex.Message
                };
            }
            
        }
        //Landlord Access
        public async Task<ResponseBase<RegisterPropertyResponseModel>> RegisterProperty(RegisterPropertyModel model, string authToken)
        {
            try
            {
                var userId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                var user = await _userManager.FindByIdAsync(userId);

                var propertyToAdd = new Property
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Address = model.Address,
                    AvailableSpaces= model.AvailableSpaces,
                    AddedAt= DateTime.UtcNow,
                    LandLord = user,
                    LandLordID = userId
                };
                _dbContext.Add(propertyToAdd);
                var result = _dbContext.SaveChanges();
                if (result == 0)
                    throw new Exception("Property did not get registered");

                return new ResponseBase<RegisterPropertyResponseModel>
                {
                    Success = true,
                    Message = "Property succesfully registered",
                    Body = new RegisterPropertyResponseModel
                    {
                        Name=propertyToAdd.Name
                    }
                };

            }
            catch (Exception ex)
            {
                return new ResponseBase<RegisterPropertyResponseModel>
                {
                    Success = false,
                    Message=ex.Message
                };
            }
            
        }
        
    }
}
