//using HomesForAll.DAL.Models.Property;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models.PropertyModels;
using HomesForAll.Utils.ServerResponse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Services.PropertyServices
{
    public interface IPropertyService
    {
        public Task<ResponseBase<List<GetPropertyResponseModel>>> GetAllProperties();
        public Task<ResponseBase<GetPropertyResponseModel>> GetProperty(string propertyId);
    }
}
