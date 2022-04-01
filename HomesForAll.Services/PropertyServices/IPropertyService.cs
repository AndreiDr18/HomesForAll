using HomesForAll.DAL.Models.Property;
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
        public Task<ResponseBase<List<GetAllPropertiesResponseModel>>> GetAllProperties();
        public Task<ResponseBase<RegisterPropertyResponseModel>> RegisterProperty(RegisterPropertyModel model, string authToken);
        public Task<ResponseBase<EmptyResponseModel>> RequestProperty(RequestPropertyModel model, string authToken);
        public Task<ResponseBase<List<GetTenantRequestsResponseModel>>> GetTenantRequests(string authToken);
        public Task<ResponseBase<EmptyResponseModel>> DeleteRequest(string authToken, string reqId);
    }
}
