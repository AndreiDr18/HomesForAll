﻿using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using HomesForAll.Services.TenantServices;

namespace HomesForAll.Services
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<ITenantService, TenantService>();
        }
    }
}