using HomesForAll.DAL;
using HomesForAll.DAL.Entities;
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
        public async Task<ResponseBase<GetPropertyResponseModel>> GetProperty(string propertyId)
        {
            try
            {
                var property = _dbContext.Properties.FirstOrDefault(p => p.Id == Guid.Parse(propertyId));
                if (property == null)
                    throw new Exception("There is no property matching the given property id");
                return new ResponseBase<GetPropertyResponseModel>
                {
                    Success = true,
                    Message = "Succesfully retrieved property",
                    Body = new GetPropertyResponseModel
                    {
                        Id = property.Id,
                        Name = property.Name,
                        Address = property.Address,
                        AvailableSpaces = property.AvailableSpaces,
                        AddedAt = property.AddedAt

                    }

                };

            }catch (Exception ex)
            {
                return new ResponseBase<GetPropertyResponseModel>
                {
                    Success = false,

                };
            }
        }
        public async Task<ResponseBase<List<GetPropertyResponseModel>>> GetAllProperties()
        {
            try
            {
                var properties = _dbContext.Properties.ToList();

                if (properties.Count == 0)
                    throw new Exception("There are no registered properties");

                List<GetPropertyResponseModel> Body = new List<GetPropertyResponseModel>();

                foreach(var property in properties)
                {
                    Body.Add(new GetPropertyResponseModel
                    {
                        Id = property.Id,
                        Name = property.Name,
                        Address = property.Address,
                        AvailableSpaces = property.AvailableSpaces,
                        AddedAt = property.AddedAt
                    });
                }
                return new ResponseBase<List<GetPropertyResponseModel>>
                {
                    Success = true,
                    Message = "Succesfully retrieved all properties",
                    Body = Body
                };
            }
            catch (Exception ex)
            {
                return new ResponseBase<List<GetPropertyResponseModel>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        
        
    }
}
