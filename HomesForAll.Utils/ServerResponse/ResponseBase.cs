using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.ServerResponse
{
    public class ResponseBase<TBody>
    {
        public bool Success { get; set; }
        public string? Message  { get; set; }
        public TBody? Body { get; set; }
    }
}
