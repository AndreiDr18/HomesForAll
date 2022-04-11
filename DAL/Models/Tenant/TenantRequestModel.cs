using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomesForAll.DAL.Entities;
using Newtonsoft.Json;

namespace HomesForAll.DAL.Models.Tenant
{
    public class TenantRequestModel
    {
        [JsonProperty("PropertyId")]
        public Guid PropertyId { get; set; }
        [JsonProperty("NumberOfPeople")]
        public int NumberOfPeople { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
        
        public bool VerifyIntegrity()
        {
            if (PropertyId == null)
                return false;
            if (NumberOfPeople <= 0)
                return false;
            if (Message == "")
                return false;
            return true;
        }
    }
}
