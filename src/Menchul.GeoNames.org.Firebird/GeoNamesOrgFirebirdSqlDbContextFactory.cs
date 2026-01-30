using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Menchul.GeoNames.org.Firebird
{
    public class GeoNamesOrgFirebirdSqlDbContextFactory : IDesignTimeDbContextFactory<GeoNamesOrgFirebirdSqlDbContext>
    {
        private readonly string? __connectionString;

        public GeoNamesOrgFirebirdSqlDbContextFactory()
        {
        }

        public GeoNamesOrgFirebirdSqlDbContextFactory(string? connectionString)
        {
            __connectionString = connectionString;
        }

        public GeoNamesOrgFirebirdSqlDbContext CreateDbContext(string[]? args)
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

            var optionsBuilder = new DbContextOptionsBuilder<GeoNamesOrgFirebirdSqlDbContext>();
            optionsBuilder.UseFirebird(connectionString);

            var geoNamesOrgDbContext = new GeoNamesOrgFirebirdSqlDbContext(optionsBuilder.Options);

            return geoNamesOrgDbContext;
        }
    }
}