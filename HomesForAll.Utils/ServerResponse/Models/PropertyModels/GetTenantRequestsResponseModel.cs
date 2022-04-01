using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.ServerResponse.Models.PropertyModels
{
    public class GetTenantRequestsResponseModel
    {
        public string RequestID { get; set; }
        public int NumberOfPeople { get; set; }
        public string Message { get; set; }
        public string PropertyID { get; set; }
        public string TenantID { get; set; }

    }
}
