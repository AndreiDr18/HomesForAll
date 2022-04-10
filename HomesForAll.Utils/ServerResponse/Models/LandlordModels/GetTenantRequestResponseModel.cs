using HomesForAll.Utils.ServerResponse.Models.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.ServerResponse.Models.LandlordModels
{
    public class GetTenantRequestResponseModel
    {
        public Guid RequestID { get; set; }
        public int NumberOfPeople { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public PropertyResponseModel Property { get; set; }
        public TenantResponseModel Tenant { get; set; }

    }
}
