using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthorization.DbService
{
    public class JWTAuthenticationDbContext : IdentityDbContext<IdentityUser>
    {
        public JWTAuthenticationDbContext(DbContextOptions<JWTAuthenticationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
