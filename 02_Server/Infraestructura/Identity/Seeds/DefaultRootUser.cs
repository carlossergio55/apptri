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
    public static class DefaultRootUser
    {
        public static async Task SeedAsync(UserManager<AplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {            
            var defaultUSer = new AplicationUser
            {
             //   Usuario = "root",                
                UserName = "root",
              //  Apellido = "root",
                Email = "sedem.sistemas@sedem.gob.bo",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            
            
            if (userManager.Users.All(u => u.Id != defaultUSer.Id)) {
                var user = await userManager.FindByNameAsync(defaultUSer.UserName);
                if (user == null) {                    
                    await userManager.CreateAsync(defaultUSer, "P4$$word");
                    await userManager.AddToRoleAsync(defaultUSer, Roles.Root.ToString());
                    await userManager.AddToRoleAsync(defaultUSer, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUSer, Roles.Basic.ToString());
                }            
            }

        }


    }
}
