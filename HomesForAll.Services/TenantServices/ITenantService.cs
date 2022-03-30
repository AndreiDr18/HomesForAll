﻿using HomesForAll.Utils.ServerResponse;
using HomesForAll.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomesForAll.Utils.ServerResponse.Models.TenantModels;
using Microsoft.AspNetCore.Http;

namespace HomesForAll.Services.TenantServices
{
    public interface ITenantService
    {
        public Task<ResponseBase<GetByIdBodyModel>> GetTenantInfo(string authToken);
    }
}
