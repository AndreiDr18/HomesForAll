using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Models.Property
{
    public class RequestPropertyModel
    {
        public string PropertyId { get; set; }
        public int NumberOfPeople { get; set; }
        public string Message { get; set; }
    }
}
