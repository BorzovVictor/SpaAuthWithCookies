using ApiCookiesAuth.Data.Entities.Identity;
using ApiCookiesAuth.Data.Entities.Maps;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiCookiesAuth.Data
{
    public class PwContext: IdentityDbContext<AppUser, AppRole, int, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public PwContext(DbContextOptions<PwContext> options): base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AppRoleClaimMap());
            builder.ApplyConfiguration(new AppRoleMap());
            builder.ApplyConfiguration(new AppUserLoginMap());
            builder.ApplyConfiguration(new AppUserMap());
            builder.ApplyConfiguration(new AppUserRoleMap());
            builder.ApplyConfiguration(new AppUserTokenMap());
            builder.ApplyConfiguration(new AppUserClaimMap());
        }
    }
}