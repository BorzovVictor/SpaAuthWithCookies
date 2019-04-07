using ApiCookiesAuth.Data.Entities.Identity;
using ApiCookiesAuth.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiCookiesAuth.Data.Entities.Maps
{
    public class AppUserTokenMap : IEntityTypeConfiguration<AppUserToken>
    {
        public void Configure(EntityTypeBuilder<AppUserToken> builder)
        {
            builder.ToTable("UserTokens", PwSettings.PwSchema);
        }
    }
}