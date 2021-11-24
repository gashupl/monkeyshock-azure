using Microsoft.EntityFrameworkCore;

namespace MonkeyShock.Azure.WebApi.Model
{
    public class AppContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {

        }
    }
}
