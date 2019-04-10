using ApiCookiesAuth.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiCookiesAuth.Data
{
    public class SimpleContext: DbContext
    {
        public SimpleContext(DbContextOptions<SimpleContext> options): base(options)
        {
           
        }

        public DbSet<User> Users { get; set; }
    }
}