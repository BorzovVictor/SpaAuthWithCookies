using ApiCookiesAuth.Data.Entities.Identity;
using ApiCookiesAuth.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiCookiesAuth.Data.Entities.Maps
{
    public class AppUserClaimMap : IEntityTypeConfiguration<AppUserClaim>
    {
        public void Configure(EntityTypeBuilder<AppUserClaim> builder)
        {
            builder.ToTable("UserClaims", PwSettings.PwSchema);
        }
    }
}