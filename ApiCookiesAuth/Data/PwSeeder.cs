using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiCookiesAuth.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace ApiCookiesAuth.Data
{
    public class PwSeeder
    {
        private readonly PwContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        private string AdminRole => "Administrator";
        public PwSeeder(PwContext db, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Seed()
        {
            _db.Database.EnsureCreated();

            var adminUser = await CreateDefaultUser();

            #region create default role

            List<string> defRoles = new List<string>
            {
                AdminRole,
                "Users"
            };

            foreach (var role in defRoles)
            {
                if(!await AddRole(role))
                    throw new InvalidOperationException($"Failed to create default role {role}!");
            }

            #endregion
            #region add default user to default role

            var userRoles = await _userManager.GetRolesAsync(adminUser);
            if (userRoles == null || !userRoles.Contains(AdminRole))
            {
                await _userManager.AddToRoleAsync(adminUser, AdminRole);
            }

            #endregion
        }

        async Task<AppUser> CreateDefaultUser()
        {
            var user = await _userManager.FindByEmailAsync("admin@pw.com");

            if (user == null)
            {
                user = new AppUser()
                {
                    UserName = "Admin",
                    Email = "admin@pw.com"
                };
                var result = await _userManager.CreateAsync(user, "Pa$$w0rd!");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Failed to create default user!");
                }

            }

            return user;
        }

        async Task<bool> AddRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new AppRole()
                {
                    Name = roleName
                };
               var result = await _roleManager.CreateAsync(role);
               return result.Succeeded;
            }
            return true;
        }
    }
}