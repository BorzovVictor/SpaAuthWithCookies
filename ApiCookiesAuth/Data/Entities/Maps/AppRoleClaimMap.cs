using ApiCookiesAuth.Data.Entities.Identity;
using ApiCookiesAuth.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiCookiesAuth.Data.Entities.Maps
{
    public class AppRoleClaimMap: IEntityTypeConfiguration<AppRoleClaim>
    {
        public void Configure(EntityTypeBuilder<AppRoleClaim> builder)
        {
            builder.ToTable("RoleClaims", PwSettings.PwSchema);
        }
    }
}