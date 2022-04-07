using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Entities
{
    public class TenantRequest : BaseEntity.BaseEntity
    {
        public int NumberOfPeople { get; set; }
        public string Message { get; set; }
        public Status Status { get; set; }
        public User Tenant { get; set; }
        public Guid TenantID { get; set; }
        public Property Property { get; set; }
        public Guid PropertyID { get; set; }
    }

    public enum Status
    {
        Pending,
        Rejected,
        Accepted
    }
}
