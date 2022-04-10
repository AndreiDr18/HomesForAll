using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.ServerResponse.Models.TenantModels
{
    public class GetAcceptedAtLandlordInfo
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime JoinedAtDate { get; set; }
        public string PhoneNumber { get; set; }
    }
}
