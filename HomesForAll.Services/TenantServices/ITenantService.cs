using HomesForAll.Utils.ServerResponse;
using HomesForAll.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomesForAll.Utils.ServerResponse.Models;
using HomesForAll.Utils.ServerResponse.Models.TenantModels;
using HomesForAll.DAL.Models.Tenant;

namespace HomesForAll.Services.TenantServices
{
    public interface ITenantService
    {
        public Task<ResponseBase<GetTenantResponseModel>> GetTenantInfo(string authToken);
        public Task<ResponseBase<EmptyResponseModel>> UpdateTenant(TenantUpdateModel model, string authToken);
        public Task<ResponseBase<EmptyResponseModel>> SendTenantRequest(TenantRequestModel model, string authToken);

        public Task<ResponseBase<List<GetRequestResponseModel>>> GetTenantRequests(string authToken);
        public Task<ResponseBase<EmptyResponseModel>> DeleteRequest(string authToken, string reqId);
        public Task<ResponseBase<GetAcceptedAtLandlordInfo>> GetLandlordInfo(string authToken);

    }
}
