using Aplicacion.Enums;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Seeds
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(UserManager<AplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUSer = new AplicationUser
            {
               // Usuario = "admin",
                UserName = "admin",
               // Apellido = "admin",
                Email = "sedem.sistemas@sedem.gob.bo",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (userManager.Users.All(u => u.Id != defaultUSer.Id))
            {
                var user = await userManager.FindByNameAsync(defaultUSer.UserName);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUSer, "123Pa$word");                    
                    await userManager.AddToRoleAsync(defaultUSer, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUSer, Roles.Basic.ToString());
                }
            }

        }

    }
}
