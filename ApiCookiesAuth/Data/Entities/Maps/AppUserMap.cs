using ApiCookiesAuth.Data.Entities.Identity;
using ApiCookiesAuth.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiCookiesAuth.Data.Entities.Maps
{
    public class AppUserMap: IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("Users", PwSettings.PwSchema);
        }
    }
}