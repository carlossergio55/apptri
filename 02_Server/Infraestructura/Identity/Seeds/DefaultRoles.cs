using Aplicacion.Enums;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<AplicationUser> userManager, RoleManager<IdentityRole> roleManager) {

            //seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Root.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
        }
    }
}
