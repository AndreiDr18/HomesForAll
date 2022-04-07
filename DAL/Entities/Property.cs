﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Entities
{
    public class Property : BaseEntity.BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int AvailableSpaces { get; set; }
        public DateTime AddedAt { get; set; }
        public User LandLord { get; set; }
        public Guid LandLordID { get; set; }
        
        public ICollection<User>? AcceptedTenants { get; set; }
        public ICollection<TenantRequest>? TenantRequests { get; set; }

    }
}
