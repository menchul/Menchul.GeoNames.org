using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Menchul.GeoNames.org.SQLite
{
    public class GeoNamesOrgSQLiteDbContextFactory : IDesignTimeDbContextFactory<GeoNamesOrgSQLiteDbContext>
    {
        private readonly string? __connectionString;

        public GeoNamesOrgSQLiteDbContextFactory()
        {
        }

        public GeoNamesOrgSQLiteDbContextFactory(string? connectionString)
        {
            __connectionString = connectionString;
        }

        public GeoNamesOrgSQLiteDbContext CreateDbContext(string[]? args = null)
        {
            string? connectionString = null;

            if (args != null && args.Length > 0)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .Build();

                connectionString = configuration["connection"];
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = __connectionString;
            }

            var optionsBuilder = new DbContextOptionsBuilder<GeoNamesOrgSQLiteDbContext>();
            optionsBuilder.UseSqlite(connectionString!);

            var geoNamesOrgDbContext = new GeoNamesOrgSQLiteDbContext(optionsBuilder.Options);

            return geoNamesOrgDbContext;
        }
    }
}