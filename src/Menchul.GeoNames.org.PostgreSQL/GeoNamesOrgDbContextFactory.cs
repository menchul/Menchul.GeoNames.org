using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Menchul.GeoNames.org.PostgreSQL
{
    public sealed class GeoNamesOrgDbContextFactory : IDesignTimeDbContextFactory<GeoNamesOrgDbContext>
    {
        public GeoNamesOrgDbContext CreateDbContext(string[] args)
        {
            string connectionString = Environment.GetEnvironmentVariable("GeoNamesOrg__ConnectionStrings");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var optionsBuilder = new DbContextOptionsBuilder<GeoNamesOrgDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            var geoNamesOrgDbContext = new GeoNamesOrgDbContext(optionsBuilder.Options);

            return geoNamesOrgDbContext;
        }
    }
}