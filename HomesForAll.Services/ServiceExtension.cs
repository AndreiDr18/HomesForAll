using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using HomesForAll.Services.AuthenticationServices;
using HomesForAll.Services.TenantServices;
using HomesForAll.Services.PropertyServices;

namespace HomesForAll.Services
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ITenantService, TenantService>();
            services.AddTransient<IPropertyService, PropertyService>();
        }
    }
}