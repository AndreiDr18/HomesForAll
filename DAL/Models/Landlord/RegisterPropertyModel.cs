using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.DAL.Models.Landlord
{
    public class RegisterPropertyModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int AvailableSpaces { get; set; }
    }
}
