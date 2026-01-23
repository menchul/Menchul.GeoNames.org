using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace Menchul.GeoNames.org.PostgreSQL
{
    public sealed class GeoNamesOrgPostgreSQLDbContextFactory : IDesignTimeDbContextFactory<GeoNamesOrgDbContext>
    {
        private readonly string? __connectionString;

        public GeoNamesOrgPostgreSQLDbContextFactory()
        {
        }

        public GeoNamesOrgPostgreSQLDbContextFactory(string? connectionString)
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
            optionsBuilder.UseNpgsql(connectionString);

            var geoNamesOrgDbContext = new GeoNamesOrgPostgreSQLDbContext(optionsBuilder.Options);

            return geoNamesOrgDbContext;
        }
    }
}