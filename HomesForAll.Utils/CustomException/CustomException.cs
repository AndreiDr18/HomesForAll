using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.CustomExceptionUtil
{
    public class CustomException : Exception
    {
        public HttpStatusCode StatusCode;
        public bool? Success;

        public CustomException(HttpStatusCode StatusCode, string message) : base(message)
        {
            this.StatusCode=StatusCode;
        }
        public CustomException(HttpStatusCode StatusCode, string message, bool success) : base(message)
        {
            this.StatusCode=StatusCode;
            this.Success=success;
        }
    }
}
