using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Models.Landlord
{
    public class RegisterPropertyModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Address")]
        public string Address { get; set; }
        [JsonProperty("AvailableSpaces")]
        public int AvailableSpaces { get; set; }
    }
}
