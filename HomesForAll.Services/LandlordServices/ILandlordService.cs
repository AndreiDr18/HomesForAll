﻿using HomesForAll.DAL.Models.Landlord;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;
using HomesForAll.Utils.ServerResponse.Models.LandlordModels;
using HomesForAll.Utils.ServerResponse.Models.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Services.LandlordServices
{
    public interface ILandlordService
    {
        public Task<ResponseBase<GetLandlordResponseModel>> GetLandlord(string authToken);
        public Task<ResponseBase<EmptyResponseModel>> UpdateLandlord(UpdateLandlordModel model, string authToken);
        public Task<ResponseBase<EmptyResponseModel>> RegisterProperty(RegisterPropertyModel model, string authToken);
        public Task<ResponseBase<EmptyResponseModel>> DeleteProperty(string propertyId, string authToken);
        public Task<ResponseBase<List<GetOwnedPropertyResponseModel>>> GetProperties(string authToken);
        public Task<ResponseBase<List<GetTenantRequestResponseModel>>> GetRequests(string authToken);
        public Task<ResponseBase<EmptyResponseModel>> AcceptRequest(string requestId, string authToken);
        public Task<ResponseBase<EmptyResponseModel>> RevokeRequest(string requestId, string authToken);
        public Task<ResponseBase<EmptyResponseModel>> EvictTenant(string authToken, string tenantId);
    }
}
