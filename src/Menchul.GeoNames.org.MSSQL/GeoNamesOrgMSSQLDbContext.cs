using Microsoft.EntityFrameworkCore;

namespace Menchul.GeoNames.org.MSSQL
{
    public class GeoNamesOrgMSSQLDbContext : GeoNamesOrgDbContext
    {
        public GeoNamesOrgMSSQLDbContext()
        {
        }

        public GeoNamesOrgMSSQLDbContext(DbContextOptions<GeoNamesOrgDbContext> options)
            : base(options)
        {
        }
    }
}