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
using HomesForAll.Utils.CustomExceptionUtil;
using System.Net;
using HomesForAll.Utils.Validators;

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
                var id = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

                #pragma warning disable
                var user = _dbContext.Users
                                     .Include(u => u.PropertyRequests)
                                     .ThenInclude(pr => pr.Property)
                                     .FirstOrDefault(u => u.Id == Guid.Parse(id));
                #pragma warning restore
                if (user == null)
                    throw new CustomException(HttpStatusCode.NotFound,"Couldn't fetch tenant info");
                if (user.PropertyRequests == null)
                    return new ResponseBase<GetTenantResponseModel>
                    {
                        Success = true,
                        Message = "No request has been registered"
                    };

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
                                Id = propertyRequest.PropertyID,
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
        public async Task<ResponseBase<EmptyResponseModel>> UpdateTenant(TenantUpdateModel model, string authToken)
        {
            if (!model.VerifyIntegrity())
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid user input");

            var id = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

            var user = await _userManager.FindByIdAsync(id);

            user.Name = model.Name;
            user.PhoneNumber = model.PhoneNumber;
            user.BirthDate = model.BirthDate;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                throw new CustomException(HttpStatusCode.InternalServerError,"User could not be updated");

            return new ResponseBase<EmptyResponseModel>
            {
                Success = true,
                Message = "User data succesfully updated"
            };
        }
        public async Task<ResponseBase<EmptyResponseModel>> SendTenantRequest(TenantRequestModel model, string authToken)
        {
            if (!model.VerifyIntegrity())
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid user input");

            #pragma warning disable
            var tenantId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

            var tenant = await _userManager.FindByIdAsync(tenantId);
            var property = _dbContext.Properties.FirstOrDefault(p => p.Id == Guid.Parse(model.PropertyId));


            if (property == null)
                throw new CustomException(HttpStatusCode.BadRequest, "No property matching given property id");

            #pragma warning restore
            if (tenant.AcceptedAtPropertyID != null)
                throw new CustomException(HttpStatusCode.Forbidden,"Tenant has already been accepted to a property");

            var requestExists = _dbContext.TenantRequests.Where(tr => tr.TenantID == Guid.Parse(tenantId) && tr.PropertyID == property.Id).Any();
            if (requestExists)
                throw new CustomException(HttpStatusCode.Forbidden,"You can't register multiple requests to the same property");

            TenantRequest tenantRequest = new TenantRequest()
            {
                Id = Guid.NewGuid(),
                NumberOfPeople = model.NumberOfPeople,
                Message = model.Message,
                Status = Status.Pending,
                Property = property,
                PropertyID = Guid.Parse(model.PropertyId),
                Tenant = tenant,
                TenantID = Guid.Parse(tenantId)
            };
            await _dbContext.TenantRequests.AddAsync(tenantRequest);

            var changes = _dbContext.SaveChanges();
            if (changes == 0)
                throw new CustomException(HttpStatusCode.InternalServerError,"Request did not get registered");

            return new ResponseBase<EmptyResponseModel>
            {
                Success = true,
                Message = "Request got succesfully registered"
            };
        }
        public async Task<ResponseBase<List<GetRequestResponseModel>>> GetTenantRequests(string authToken)
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
                        Id = tenantRequest.Id,
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
        public async Task<ResponseBase<EmptyResponseModel>> DeleteRequest(string authToken, string reqId)
        {
            if (!GuidValidator.IsGuid(reqId))
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid request id");
            var tenantRequest = _dbContext.TenantRequests.Where(tr => tr.Id == Guid.Parse(reqId)).FirstOrDefault();
            var tenantId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");

            if (tenantRequest == null)
                throw new CustomException(HttpStatusCode.BadRequest,"There is no request matching the given id");
            if (tenantRequest.Status == Status.Accepted)
                throw new CustomException(HttpStatusCode.Forbidden,"The request has already been accepted");
            if (tenantRequest.TenantID != Guid.Parse(tenantId))
                throw new CustomException(HttpStatusCode.Forbidden,"You can only delete requests that are your own");
            if (tenantRequest.Status == Status.Rejected)
                throw new CustomException(HttpStatusCode.Forbidden, "The request has already been rejected");

            _dbContext.TenantRequests.Remove(tenantRequest);
            var changes = await _dbContext.SaveChangesAsync();

            if (changes == 0)
                throw new CustomException(HttpStatusCode.InternalServerError,"Could not remove tenant request");

            return new ResponseBase<EmptyResponseModel>
            {
                Success = true,
                Message = "Succesfully removed tenant request"
            };

        }
        public async Task<ResponseBase<GetAcceptedAtLandlordInfo>> GetLandlordInfo(string authToken)
        {
            var userId = TokenManager.ExtractHeaderValueJWT(authToken, "UserId");
            var user = _dbContext.Users
                                    .Include(u => u.AcceptedAtProperty)
                                    .ThenInclude(aap => aap.LandLord)
                                    .FirstOrDefault(u => u.Id == Guid.Parse(userId));

            if (user.AcceptedAtProperty == null)
                throw new CustomException(HttpStatusCode.BadRequest,"User has not been accepted to any property yet");

            return new ResponseBase<GetAcceptedAtLandlordInfo>
            {
                Success = true,
                Message = "Succesfully retrieved landlord info",
                Body = new GetAcceptedAtLandlordInfo
                {
                    Name = user.AcceptedAtProperty.LandLord.Name,
                    PhoneNumber = user.AcceptedAtProperty.LandLord.PhoneNumber,
                    JoinedAtDate = user.AcceptedAtProperty.LandLord.JoinedAtDate,
                    BirthDate = user.AcceptedAtProperty.LandLord.BirthDate
                }
            };
        }
    }
}
