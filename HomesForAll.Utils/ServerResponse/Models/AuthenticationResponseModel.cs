﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.ServerResponse.Models
{
    public class AuthenticationResponseModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
