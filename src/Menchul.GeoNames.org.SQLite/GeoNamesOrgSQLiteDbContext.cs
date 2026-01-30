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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Firebird does not support schemas; clear schema for all entity types
            modelBuilder.HasDefaultSchema(null);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetSchema(null);
            }
        }
    }
}