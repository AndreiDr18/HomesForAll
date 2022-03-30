using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using HomesForAll.DAL.Entities;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace HomesForAll.DAL.UserRoles
{
    public class RoleSeeder
    {
        public static async Task InitRoles(IServiceScope _scope)
        {
            try
            {

                var _roleManager = _scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var roles = typeof(Roles).GetFields();

                foreach (var role in roles)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(role.Name);
                    if (!roleExists)
                    {
                        await CreateRoleAsync(role.Name, _roleManager);
                    }
                }
            }
            catch (Exception ex)
            {
                //logger
                Console.WriteLine(ex.Message);
            }

        }

        private static async Task CreateRoleAsync(string roleName, RoleManager<IdentityRole> _roleManager)
        {
            var roleToCreate = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(roleToCreate);
            if (!result.Succeeded)
            {
                throw new Exception("Role " + roleName + " could not be created");
            }

        }
    }
}
    


