using ApiCookiesAuth.Data.Entities.Identity;
using ApiCookiesAuth.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiCookiesAuth.Data.Entities.Maps
{
    public class AppRoleMap : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            builder.ToTable("Roles", PwSettings.PwSchema);
        }
    }
}