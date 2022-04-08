using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomesForAll.DAL.Entities;

namespace HomesForAll.Utils.ServerResponse.Models.TenantModels
{
    public class TenantRequestResponseModel
    {

        public Guid RequestID { get; set; }
        public int NumberOfPeople { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public PropertyResponseModel Property { get; set; }

    }
}
