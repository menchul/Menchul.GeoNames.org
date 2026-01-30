using Microsoft.EntityFrameworkCore;

namespace Menchul.GeoNames.org.Firebird
{
    public class GeoNamesOrgFirebirdSqlDbContext : GeoNamesOrgDbContext
    {
        public GeoNamesOrgFirebirdSqlDbContext()
        {
        }

        public GeoNamesOrgFirebirdSqlDbContext(DbContextOptions<GeoNamesOrgFirebirdSqlDbContext> options)
            : base(options)
        {
        }
    }
}