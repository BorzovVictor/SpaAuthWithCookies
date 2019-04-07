using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ApiCookiesAuth.Data.Entities.Identity
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}