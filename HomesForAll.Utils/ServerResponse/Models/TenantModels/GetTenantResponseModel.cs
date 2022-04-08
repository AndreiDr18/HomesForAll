using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.ServerResponse.Models.TenantModels
{
    public class GetTenantResponseModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Username { get; set; }
        public List<TenantRequestResponseModel> PropertyRequests { get; set; }
        
    }

}
