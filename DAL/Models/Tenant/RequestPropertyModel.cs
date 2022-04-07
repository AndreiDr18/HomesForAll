using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomesForAll.DAL.Entities;

namespace HomesForAll.DAL.Models.Tenant
{
    public class RequestPropertyModel
    {
        public Guid PropertyId { get; set; }
        public int NumberOfPeople { get; set; }
        public string Message { get; set; }
    }
}
