using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Menchul.GeoNames.org.MSSQL
{
    public class GeoNamesOrgMSSQLDbContextFactory : IDesignTimeDbContextFactory<GeoNamesOrgDbContext>
    {
        private readonly string? __connectionString;

        public GeoNamesOrgMSSQLDbContextFactory()
        {
        }

        public GeoNamesOrgMSSQLDbContextFactory(string? connectionString)
        {
            __connectionString = connectionString;
        }

        public GeoNamesOrgDbContext CreateDbContext(string[]? args = null)
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

            var optionsBuilder = new DbContextOptionsBuilder<GeoNamesOrgDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var geoNamesOrgDbContext = new GeoNamesOrgMSSQLDbContext(optionsBuilder.Options);

            return geoNamesOrgDbContext;
        }
    }
}