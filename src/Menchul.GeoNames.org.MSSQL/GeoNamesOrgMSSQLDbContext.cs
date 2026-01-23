using Microsoft.EntityFrameworkCore;

namespace Menchul.GeoNames.org.MSSQL
{
    public class GeoNamesOrgMSSQLDbContext : GeoNamesOrgDbContext
    {
        public GeoNamesOrgMSSQLDbContext()
        {
        }

        public GeoNamesOrgMSSQLDbContext(DbContextOptions<GeoNamesOrgMSSQLDbContext> options)
            : base(options)
        {
        }
    }
}