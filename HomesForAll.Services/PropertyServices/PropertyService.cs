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
using HomesForAll.Utils.CustomExceptionUtil;
using System.Net;

namespace HomesForAll.Services.PropertyServices
{
    public class PropertyService : IPropertyService
    {
        private readonly AppDbContext _dbContext;
        public PropertyService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseBase<GetPropertyResponseModel>> GetProperty(string propertyId)
        {
            var property = _dbContext.Properties.FirstOrDefault(p => p.Id == Guid.Parse(propertyId));
            if (propertyId == null)
                throw new CustomException(HttpStatusCode.BadRequest, "Invalid property id");
            if (property == null)
                throw new CustomException(HttpStatusCode.NotFound,"There is no property matching the given property id");
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
        }
        public async Task<ResponseBase<List<GetPropertyResponseModel>>> GetAllProperties()
        {
                var properties = _dbContext.Properties.ToList();

                if (properties.Count == 0)
                    throw new CustomException(HttpStatusCode.OK,"There are no registered properties");

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
    }
}
