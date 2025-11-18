using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task3_V7.Data;

namespace Task3_V7.Data
{
    public class AppAdContext: IdentityDbContext<AppUser>

    {
        public AppAdContext(DbContextOptions<AppAdContext> options)
            : base(options)
        {
        }
    }
}
