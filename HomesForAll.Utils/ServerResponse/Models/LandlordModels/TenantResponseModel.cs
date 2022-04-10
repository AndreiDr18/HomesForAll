using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.ServerResponse.Models.LandlordModels
{
    public class TenantResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
