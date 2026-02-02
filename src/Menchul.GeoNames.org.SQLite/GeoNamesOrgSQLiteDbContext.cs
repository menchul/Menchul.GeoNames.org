using Microsoft.EntityFrameworkCore;

namespace Menchul.GeoNames.org.SQLite
{
    public class GeoNamesOrgSQLiteDbContext : GeoNamesOrgDbContext
    {
        public GeoNamesOrgSQLiteDbContext()
        {
        }

        public GeoNamesOrgSQLiteDbContext(DbContextOptions<GeoNamesOrgSQLiteDbContext> options)
            : base(options)
        {
        }
    }
}