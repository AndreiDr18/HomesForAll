using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomesForAll.DAL.Models.Tenant;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;

namespace HomesForAll.Services.TenantServices
{
    public interface ITenantService
    {
        public Task<ResponseBase<TenantRegistrationBodyModel>> RegisterTenant(TenantRegisterModel model);
    }
}
