using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Menchul.GeoNames.org.PostgreSQL
{
    public sealed class GeoNamesOrgPostgreSQLDbContextFactory : IDesignTimeDbContextFactory<GeoNamesOrgPostgreSQLDbContext>
    {
        private readonly string? __connectionString;

        public GeoNamesOrgPostgreSQLDbContextFactory()
        {
        }

        public GeoNamesOrgPostgreSQLDbContextFactory(string? connectionString)
        {
            __connectionString = connectionString;
        }

        public GeoNamesOrgPostgreSQLDbContext CreateDbContext(string[]? args = null)
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

            var optionsBuilder = new DbContextOptionsBuilder<GeoNamesOrgPostgreSQLDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            var geoNamesOrgDbContext = new GeoNamesOrgPostgreSQLDbContext(optionsBuilder.Options);

            return geoNamesOrgDbContext;
        }
    }
}