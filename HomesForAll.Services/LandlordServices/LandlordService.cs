using HomesForAll.DAL;
using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Landlord;
using HomesForAll.Utils.JWT;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.LandlordModels;
using HomesForAll.Utils.ServerResponse.Models;
using Microsoft.AspNetCore.Identity;

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
        public async Task<ResponseBase<List<GetRequestResponseModel>>> GetRequests(string authToken)
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

                List<GetRequestResponseModel> matchedRequests = new List<GetRequestResponseModel>();
                foreach(var property in landlordProperties)
                {
                    foreach(var request in allRequests)
                    {
                        if (request.PropertyID == property.Id)
                            matchedRequests.Add(new GetRequestResponseModel
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
                return new ResponseBase<List<GetRequestResponseModel>>
                {
                    Success = true,
                    Message = "Sucesfully retrieved requests",
                    Body = matchedRequests
                };
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<GetRequestResponseModel>>
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
        public async Task<ResponseBase<EmptyResponseModel>> AcceptRequest(string requestId)
        {
            try
            {
                var tenantRequest = _dbContext.TenantRequests.FirstOrDefault(tr => tr.Id == Guid.Parse(requestId));
                if (tenantRequest == null)
                    throw new Exception("The request id is invalid");
                if (tenantRequest.Property.AvailableSpaces < tenantRequest.NumberOfPeople)
                    throw new Exception("Not enough available spaces");

                tenantRequest.Status = Status.Accepted;

                tenantRequest.Tenant.AcceptedAtPropertyID = tenantRequest.PropertyID;
                tenantRequest.Tenant.AcceptedAtProperty = tenantRequest.Property;
                
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


    }
}
