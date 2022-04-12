using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.Validators
{
    public class GuidValidator
    {
        public static bool IsGuid(string input)
        {
            if (Guid.TryParse(input, out Guid guid))
                return true;
            return false;
        }
    }
}
