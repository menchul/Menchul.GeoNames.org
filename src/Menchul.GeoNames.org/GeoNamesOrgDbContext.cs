using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;

namespace Menchul.GeoNames.org
{
    public abstract class GeoNamesOrgDbContext : DbContext
    {
        public const string DBSchema = "gno";

        protected GeoNamesOrgDbContext()
        {
        }

        protected GeoNamesOrgDbContext(DbContextOptions<GeoNamesOrgDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeoNamesOrgDbContext).Assembly);
        }


        public DbSet<ISOLanguage>? ISOLanguages { get; set; }

        public DbSet<FeatureClass>? FeatureClasses { get; set; }

        public DbSet<FeatureCode>? FeatureCodes { get; set; }

        public DbSet<FeatureCodeName>? FeatureCodeNames { get; set; }

        public DbSet<Continent>? Continents { get; set; }

        public DbSet<Country>? Countries { get; set; }

        public DbSet<TimeZone>? TimeZones { get; set; }

        public DbSet<GeoName>? GeoNames { get; set; }

        public DbSet<AlternateNameV2>? AlternateNamesV2 { get; set; }
    }
}