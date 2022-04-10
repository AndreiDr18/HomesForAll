using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Models.Authentication
{
    public class RegistrationModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [JsonProperty("Username")]
        public string Username { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("Role")]
        public string Role { get; set; }
        [JsonProperty("BirthDate")]
        public DateTime BirthDate { get; set; }
    }
}
