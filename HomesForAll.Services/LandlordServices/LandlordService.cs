﻿using HomesForAll.DAL;
using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Landlord;
using HomesForAll.Utils.JWT;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.LandlordModels;
using HomesForAll.Utils.ServerResponse.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomesForAll.Services.LandlordServices
{
    public class LandlordService : ILandlordService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public LandlordService(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ResponseBase<RegisterPropertyResponseModel>> RegisterProperty(RegisterPropertyModel model, string authToken)
        {
            try
            {
                var userId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                var user = await _userManager.FindByIdAsync(userId);

                var propertyToAdd = new Property
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Address = model.Address,
                    AvailableSpaces= model.AvailableSpaces,
                    AddedAt= DateTime.UtcNow,
                    LandLord = user,
                    LandLordID = Guid.Parse(userId)
                };
                _dbContext.Add(propertyToAdd);
                var changes = _dbContext.SaveChanges();
                if (changes == 0)
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
        public async Task<ResponseBase<List<GetTenantRequestResponseModel>>> GetRequests(string authToken)
        {
            try
            {
                var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                if (landlordId == null)
                    throw new Exception("Invalid authorization");
                var landlordProperties = _dbContext.Properties.Where(p => p.LandLordID == Guid.Parse(landlordId)).ToList();
                var allRequests = _dbContext.TenantRequests.ToList();
                if (landlordProperties.Count == 0)
                    throw new Exception("Landlord has no registered properties");
                if (allRequests.Count == 0)
                    throw new Exception("There are no registered requests");

                List<GetTenantRequestResponseModel> matchedRequests = new List<GetTenantRequestResponseModel>();
                foreach(var property in landlordProperties)
                {
                    foreach(var request in allRequests)
                    {
                        if (request.PropertyID == property.Id)
                            matchedRequests.Add(new GetTenantRequestResponseModel
                            {
                                RequestID = request.Id,
                                NumberOfPeople = request.NumberOfPeople,
                                Message = request.Message,
                                Status = request.Status.ToString(),
                                PropertyID = property.Id,
                                TenantID = request.TenantID
                            });
                    }
                }
                return new ResponseBase<List<GetTenantRequestResponseModel>>
                {
                    Success = true,
                    Message = "Sucesfully retrieved requests",
                    Body = matchedRequests
                };
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<GetTenantRequestResponseModel>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ResponseBase<GetLandlordResponseModel>> GetLandlord(string authToken)
        {
            try
            {
                var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                var landlord = await _userManager.FindByIdAsync(landlordId);

                if (landlord == null || landlordId == null)
                    throw new Exception("Landlord data could not be fetched");

                return new ResponseBase<GetLandlordResponseModel>
                {
                    Success = true,
                    Message = "Succesfully retrieved landlord data",
                    Body = new GetLandlordResponseModel
                    {
                        Name = landlord.Name,
                        PhoneNumber = landlord.PhoneNumber,
                        BirthDate = landlord.BirthDate,
                        JoinedAtDate = landlord.JoinedAtDate
                    }
                };

            }catch (Exception ex)
            {
                return new ResponseBase<GetLandlordResponseModel>
                {
                    Success = false,
                    Message=ex.Message
                };
            }
        }
        public async Task<ResponseBase<EmptyResponseModel>> UpdateLandlord(UpdateLandlordModel model, string authToken)
        {
            try
            {
                var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                var landlord = await _userManager.FindByIdAsync(landlordId);

                if (landlord == null)
                    throw new Exception("Landlord data couldn't be fetched");

                landlord.Name = model.Name;
                landlord.PhoneNumber = model.PhoneNumber;
                landlord.BirthDate = model.BirthDate;

                var changes = await _dbContext.SaveChangesAsync();
                if (changes == 0)
                    throw new Exception("User was not updated.");

                return new ResponseBase<EmptyResponseModel>
                {
                    Success = true,
                    Message = "Landlord data has been succesfully updated"
                };
                

            }
            catch (Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success = false,
                    Message=ex.Message
                };
            }
        }
        public async Task<ResponseBase<EmptyResponseModel>> AcceptRequest(string requestId, string authToken)
        {
            try
            {
                var tenantRequest = _dbContext.TenantRequests
                                               .Include(tr => tr.Property)
                                               .Include(tr => tr.Property.AcceptedTenants)
                                               .Include(tr => tr.Tenant)
                                               .Include(tr => tr.Tenant.PropertyRequests)
                                               .FirstOrDefault(tr => tr.Id == Guid.Parse(requestId));

                var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                if (tenantRequest == null)
                    throw new Exception("The request id is invalid");
                if (tenantRequest.Property.AvailableSpaces < tenantRequest.NumberOfPeople)
                    throw new Exception("Not enough available spaces");
                if (tenantRequest.Property.LandLordID != Guid.Parse(landlordId))
                    throw new Exception("You can only accept requests made to your properties");

                tenantRequest.Status = Status.Accepted;

                tenantRequest.Tenant.AcceptedAtPropertyID = tenantRequest.PropertyID;
                
                foreach(var propertyRequest in tenantRequest.Tenant.PropertyRequests)
                {
                    if(propertyRequest.Id != Guid.Parse(requestId))
                    {
                        tenantRequest.Tenant.PropertyRequests.Remove(propertyRequest);
                    }
                }

                tenantRequest.Property.AvailableSpaces -= tenantRequest.NumberOfPeople;
                tenantRequest.Property.AcceptedTenants.Add(tenantRequest.Tenant);
                

                var changes = await _dbContext.SaveChangesAsync();
                if(changes == 0)
                {
                    throw new Exception("Request couldn't be accepted");
                }
                return new ResponseBase<EmptyResponseModel>
                {
                    Success= true,
                    Message="Succesfully accepted request"
                };



            }catch (Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success=false,
                    Message=ex.Message
                };
            }
        }
        public async Task<ResponseBase<EmptyResponseModel>> RevokeRequest(string requestId, string authToken)
        {
            try
            {
                var tenantRequest = _dbContext.TenantRequests
                                              .Include(tr => tr.Property)
                                              .Include(tr => tr.Tenant)
                                              .FirstOrDefault(tr => tr.Id == Guid.Parse(requestId));

                var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                if (tenantRequest == null)
                    throw new Exception("Invalid request");

                if(tenantRequest.Property.LandLordID != Guid.Parse(landlordId))
                    throw new Exception("The request has not been made to one of your properties");

                if(tenantRequest.Tenant.AcceptedAtPropertyID == tenantRequest.PropertyID)
                    throw new Exception("Tenant has already been accepted, you must evict him instead.");

                _dbContext.TenantRequests.Remove(tenantRequest);

                var changes = await _dbContext.SaveChangesAsync();
                if (changes == 0)
                    throw new Exception("Couldn't revoke the request");

                return new ResponseBase<EmptyResponseModel>
                {
                    Success = true,
                    Message= "Succesfully revoked the request"
                };

            }catch (Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success = false,
                    Message=ex.Message
                };
            }
        }
        public async Task<ResponseBase<EmptyResponseModel>> DeleteProperty(string propertyId, string authToken)
        {
            try
            {
                var propertyToDelete = _dbContext.Properties.FirstOrDefault(p => p.Id == Guid.Parse(propertyId));

                if (propertyToDelete == null)
                    throw new Exception("Invalid property ID");

                var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
                var acceptedTenants = _dbContext.Users.Where(u => u.AcceptedAtPropertyID == propertyToDelete.Id).ToList();

                if (propertyToDelete.LandLordID != Guid.Parse(landlordId))
                    throw new Exception("You can only delete your own properties");

                _dbContext.Properties.Remove(propertyToDelete);
                await _dbContext.SaveChangesAsync();

                return new ResponseBase<EmptyResponseModel>
                {
                    Success=true,
                    Message = "Succesfully deleted the property"
                };
            }catch(Exception ex)
            {
                return new ResponseBase<EmptyResponseModel>
                {
                    Success= false,
                    Message = ex.Message
                };
            }
        }
        //Evict tenant
    }
}
