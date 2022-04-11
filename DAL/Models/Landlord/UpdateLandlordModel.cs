using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Models.Landlord
{
    public class UpdateLandlordModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("BirthDate")]
        public DateTime BirthDate { get; set; }

        public bool VerifyIntegrity()
        {
            if (this.Name == "" || this.Name == null)
                return false;
            if (this.PhoneNumber == "" || this.PhoneNumber == null)
                return false;
            return true;
        }
    }
}
