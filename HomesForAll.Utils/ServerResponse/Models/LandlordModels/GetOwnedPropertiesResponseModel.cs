using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.ServerResponse.Models.LandlordModels
{
    public class GetOwnedPropertyResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int AvailableSpaces { get; set; }
        public DateTime AddedAt { get; set; }
        public List<TenantResponseModel>? AcceptedTenants { get; set; }

    }
}
