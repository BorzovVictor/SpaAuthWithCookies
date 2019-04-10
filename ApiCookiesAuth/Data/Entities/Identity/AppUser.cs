using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ApiCookiesAuth.Data.Entities.Identity
{
    public class AppUser: IdentityUser<int>
    {
        public decimal CurrentBalance { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}