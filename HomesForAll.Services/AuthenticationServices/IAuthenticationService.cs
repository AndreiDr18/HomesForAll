﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomesForAll.DAL.Models.Authentication;
using HomesForAll.Utils.ServerResponse;
using HomesForAll.Utils.ServerResponse.Models;

namespace HomesForAll.Services.AuthenticationServices
{
    public interface IAuthenticationService
    {
        public Task<ResponseBase<AuthenticationResponseModel>> Register(RegistrationModel model);
        public Task<ResponseBase<AuthenticationResponseModel>> Login(LoginModel model);
    }
}