using Microsoft.EntityFrameworkCore;

namespace Menchul.GeoNames.org.PostgreSQL
{
    public class GeoNamesOrgPostgreSQLDbContext : GeoNamesOrgDbContext
    {
        public GeoNamesOrgPostgreSQLDbContext()
        {
        }

        public GeoNamesOrgPostgreSQLDbContext(DbContextOptions<GeoNamesOrgDbContext> options)
            : base(options)
        {
        }
    }
}