using HomesForAll.DAL;
using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Landlord;
using HomesForAll.Utils.JWT;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.LandlordModels;
using HomesForAll.Utils.ServerResponse.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HomesForAll.Utils.CustomExceptionUtil;
using System.Net;
using HomesForAll.Utils.ServerResponse.Models.TenantModels;
using HomesForAll.Utils.Validators;

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

        public async Task<ResponseBase<EmptyResponseModel>> RegisterProperty(RegisterPropertyModel model, string authToken)
        {
            if (!model.VerifyIntegrity())
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid user input");

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
                throw new CustomException(HttpStatusCode.InternalServerError,"Property did not get registered");

            return new ResponseBase<EmptyResponseModel>
            {
                Success = true,
                Message = "Property succesfully registered"
            };

        }
        public async Task<ResponseBase<EmptyResponseModel>> DeleteProperty(string propertyId, string authToken)
        {
            if (!GuidValidator.IsGuid(propertyId))
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid property id");
#pragma warning disable CS8604
            var propertyToDelete = _dbContext.Properties
                                             .FirstOrDefault(p => p.Id == Guid.Parse(propertyId));
                
            if (propertyToDelete == null)
                throw new CustomException(HttpStatusCode.BadRequest,"Invalid property ID");

            var acceptedTenants = _dbContext.Users.Where(u => u.AcceptedAtPropertyID == propertyToDelete.Id).ToList();
#pragma warning restore CS8604

            var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
            if (propertyToDelete.LandLordID != Guid.Parse(landlordId))
                throw new CustomException(HttpStatusCode.Unauthorized,"You can only delete your own properties");

            _dbContext.Properties.Remove(propertyToDelete);
            await _dbContext.SaveChangesAsync();

            return new ResponseBase<EmptyResponseModel>
            {
                Success=true,
                Message = "Succesfully deleted the property"
            };
        }
        public async Task<ResponseBase<List<GetOwnedPropertyResponseModel>>> GetProperties(string authToken)
        {
            var userId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

            var user = _dbContext.Users
                                 .Include(u => u.Properties).ThenInclude(p => p.AcceptedTenants)
                                 .FirstOrDefault(u => u.Id == Guid.Parse(userId));

            if ( user.Properties == null || user.Properties.Count == 0)
                return new ResponseBase<List<GetOwnedPropertyResponseModel>>
                {
                    Success=true,
                    Message = "User has no properties"
                };

            var properties = new List<GetOwnedPropertyResponseModel>();

            foreach (var property in user.Properties)
            {
                var acceptedTenants = new List<TenantResponseModel>();
                foreach(var AcceptedTenant in property.AcceptedTenants)
                {
                    acceptedTenants.Add(new TenantResponseModel
                    {
                        Id = AcceptedTenant.Id,
                        Name = AcceptedTenant.Name,
                        PhoneNumber = AcceptedTenant.PhoneNumber,
                        DateOfBirth = AcceptedTenant.BirthDate,
                        JoinedAtDate = AcceptedTenant.JoinedAtDate
                    });
                }
                properties.Add(new GetOwnedPropertyResponseModel
                {
                    Id = property.Id,
                    Name = property.Name,
                    Address = property.Address,
                    AvailableSpaces = property.AvailableSpaces,
                    AddedAt = property.AddedAt,
                    AcceptedTenants = acceptedTenants
                });
                


            }

            return new ResponseBase<List<GetOwnedPropertyResponseModel>>
            {
                Success = true,
                Message = "Sucesfully retrieved owned properties",
                Body = properties
            };
        }
        public async Task<ResponseBase<GetLandlordResponseModel>> GetLandlord(string authToken)
        {
            var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
            var landlord = await _userManager.FindByIdAsync(landlordId);

            if (landlord == null || landlordId == null)
                throw new CustomException(HttpStatusCode.BadRequest,"Landlord data could not be fetched");

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
        }
        public async Task<ResponseBase<EmptyResponseModel>> UpdateLandlord(UpdateLandlordModel model, string authToken)
        {
            if (!model.VerifyIntegrity())
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid user input");

            var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
            var landlord = await _userManager.FindByIdAsync(landlordId);

            if (landlord.Name == model.Name && landlord.PhoneNumber == model.PhoneNumber && landlord.BirthDate == model.BirthDate)
                return new ResponseBase<EmptyResponseModel>
                {
                    Success = true,
                    Message = "No changes have been made"
                };

            landlord.Name = model.Name;
            landlord.PhoneNumber = model.PhoneNumber;
            landlord.BirthDate = model.BirthDate;

            var changes = await _dbContext.SaveChangesAsync();
            if (changes == 0)
                throw new CustomException(HttpStatusCode.InternalServerError,"User was not updated.");

            return new ResponseBase<EmptyResponseModel>
            {
                Success = true,
                Message = "Landlord data has been succesfully updated"
            };
               
        }
        public async Task<ResponseBase<EmptyResponseModel>> AcceptRequest(string requestId, string authToken)
        {
            if (!GuidValidator.IsGuid(requestId))
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid request id");
#pragma warning disable
            var tenantRequest = _dbContext.TenantRequests
                                            .Include(tr => tr.Property)
                                            .Include(tr => tr.Property.AcceptedTenants)
                                            .Include(tr => tr.Tenant)
                                            .Include(tr => tr.Tenant.PropertyRequests)
                                            .FirstOrDefault(tr => tr.Id == Guid.Parse(requestId));

            if (tenantRequest == null)
                throw new CustomException(HttpStatusCode.NotFound,"The request id is invalid");
            if (tenantRequest.Status == Status.Rejected)
                throw new CustomException(HttpStatusCode.BadRequest, "The request has already been rejected");

            var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
            if (tenantRequest.Status == Status.Accepted)
                throw new CustomException(HttpStatusCode.Forbidden,"Request has already been accepted");
            if (tenantRequest.Property.AvailableSpaces < tenantRequest.NumberOfPeople)
                throw new CustomException(HttpStatusCode.Forbidden,"Not enough available spaces");
            if (tenantRequest.Property.LandLordID != Guid.Parse(landlordId))
                throw new CustomException(HttpStatusCode.Forbidden,"You can only accept requests made to your properties");

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

#pragma warning restore
            var changes = await _dbContext.SaveChangesAsync();
            if(changes == 0)
            {
                throw new CustomException(HttpStatusCode.InternalServerError,"Request couldn't be accepted");
            }
            return new ResponseBase<EmptyResponseModel>
            {
                Success= true,
                Message="Succesfully accepted request"
            };
        }
        public async Task<ResponseBase<EmptyResponseModel>> RevokeRequest(string requestId, string authToken)
        {
            if (!GuidValidator.IsGuid(requestId))
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid request id");
#pragma warning disable
            var tenantRequest = _dbContext.TenantRequests
                                            .Include(tr => tr.Property)
                                            .Include(tr => tr.Tenant)
                                            .FirstOrDefault(tr => tr.Id == Guid.Parse(requestId));
#pragma warning restore
            var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
            if (tenantRequest == null)
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid request");

            if(tenantRequest.Property.LandLordID != Guid.Parse(landlordId))
                throw new CustomException(HttpStatusCode.Forbidden,"The request has not been made to one of your properties");

            if(tenantRequest.Tenant.AcceptedAtPropertyID == tenantRequest.PropertyID)
                throw new CustomException(HttpStatusCode.Forbidden,"Tenant has already been accepted, you must evict him instead.");

            tenantRequest.Status = Status.Rejected;

            var changes = await _dbContext.SaveChangesAsync();
            if (changes == 0)
                throw new CustomException(HttpStatusCode.InternalServerError,"Couldn't revoke the request");

            return new ResponseBase<EmptyResponseModel>
            {
                Success = true,
                Message= "Succesfully revoked the request"
            };

        }
        public async Task<ResponseBase<List<GetTenantRequestResponseModel>>> GetRequests(string authToken)
        {
            var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

#pragma warning disable
            var landlordProperties = await _dbContext.Properties
                                                        .Include(p => p.TenantRequests).ThenInclude(p => p.Tenant)
                                                        .Where(p => p.LandLordID == Guid.Parse(landlordId))
                                                        .ToListAsync();
#pragma warning restore
            if (landlordProperties.Count == 0)
                return new ResponseBase<List<GetTenantRequestResponseModel>>
                {
                    Success = true,
                    Message = "Landlord has no registered properties"
                };

            List<GetTenantRequestResponseModel> requests = new List<GetTenantRequestResponseModel>();
            foreach (var property in landlordProperties)
            {
                if (property.TenantRequests == null) continue;

                foreach (var request in property.TenantRequests)
                    requests.Add(new GetTenantRequestResponseModel
                    {
                        RequestID = request.Id,
                        NumberOfPeople = request.NumberOfPeople,
                        Message = request.Message,
                        Status = request.Status.ToString(),
                        Tenant = new TenantResponseModel
                        {
                            Id = request.Tenant.Id,
                            Name = request.Tenant.Name,
                            PhoneNumber = request.Tenant.PhoneNumber,
                            DateOfBirth = request.Tenant.BirthDate
                        },
                        Property = new Utils.ServerResponse.Models.TenantModels.PropertyResponseModel
                        {
                            Id = property.Id,
                            Name= property.Name,
                            Address = property.Address,
                            AvailableSpaces = property.AvailableSpaces,
                            AddedAt = property.AddedAt
                        }


                    });
            }
            return new ResponseBase<List<GetTenantRequestResponseModel>>
            {
                Success = true,
                Message = "Sucesfully retrieved requests",
                Body = requests
            };
        }

        public async Task<ResponseBase<EmptyResponseModel>> EvictTenant(string authToken, string tenantId)
        {
            if (!GuidValidator.IsGuid(tenantId))
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid tenant id");

            var landlordId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

            var tenantRequest = _dbContext.TenantRequests
                                     .Include(tr => tr.Property).ThenInclude(p => p.AcceptedTenants)
                                     .FirstOrDefault(tr => tr.TenantID == Guid.Parse(tenantId) && tr.Property.LandLordID == Guid.Parse(landlordId));
            var tenant = _dbContext.Users.FirstOrDefault(u => u.Id == Guid.Parse(tenantId));

            if (tenantRequest == null || tenant == null)
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid tenantId");

            tenantRequest.Property.AcceptedTenants.Remove(tenant);
            tenant.AcceptedAtProperty = null;
            _dbContext.TenantRequests.Remove(tenantRequest);
            var changes = await _dbContext.SaveChangesAsync();

            if (changes == 0)
                throw new CustomException(HttpStatusCode.InternalServerError, "Eviction request couldn't be completed due to a server error");

            return new ResponseBase<EmptyResponseModel>
            {
                Success = true,
                Message = "Tenant succesfully evicted"
            };
        }
    }
}
