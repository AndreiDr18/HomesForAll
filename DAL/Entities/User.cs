using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime JoinedAtDate { get; set; }
        //LandLord access level
        public ICollection<Property>? Properties { get; set; }

        //Tenant access level
        public Property? AcceptedAtProperty { get; set; }
        public ICollection<Property>? RequestedProperties { get; set; }
    }
}
