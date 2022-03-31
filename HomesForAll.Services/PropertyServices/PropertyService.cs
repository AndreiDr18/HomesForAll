using HomesForAll.DAL;
using HomesForAll.DAL.Entities;
using HomesForAll.DAL.Models.Property;
using HomesForAll.Utils.JWT;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.PropertyModels;
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
