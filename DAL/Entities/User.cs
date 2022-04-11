using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public DateTime JoinedAtDate { get; set; }
        //LandLord role
        public ICollection<Property>? Properties { get; set; }

        //Tenant role
        public Property? AcceptedAtProperty { get; set; }
        public Guid? AcceptedAtPropertyID { get; set; }
        public ICollection<TenantRequest>? PropertyRequests { get; set; }

        //Auth
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryDate { get; set; }
    }
}
