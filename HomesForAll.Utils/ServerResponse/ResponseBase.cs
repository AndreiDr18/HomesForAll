using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HomesForAll.Utils.ServerResponse
{
    public class ResponseBase<TBody>
    {
        public bool Success { get; set; }
        public string? Message  { get; set; }
        public TBody? Body { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
