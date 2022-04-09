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
        public Guid PropertyID { get; set; }
        public Guid TenantID { get; set; }

    }
}
