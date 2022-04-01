using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Entities
{
    public class TenantRequest
    {
        public string Id { get; set; }
        public int NumberOfPeople { get; set; }
        public string Message { get; set; }
        public User Tenant { get; set; }
        public string TenantID { get; set; }
        public Property Property { get; set; }
        public string PropertyID { get; set; }
    }
}
