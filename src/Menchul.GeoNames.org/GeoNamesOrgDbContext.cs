using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;

namespace Menchul.GeoNames.org;

public abstract class GeoNamesOrgDbContext : DbContext
{
    public const string DBSchema = "gno";

    public static class TableNames
    {
        public const string Continents = "Continents";
        public const string Countries = "Countries";
        public const string FeatureClasses = "FeatureClasses";
        public const string FeatureCodes = "FeatureCodes";
        public const string FeatureCodeNames = "FeatureCodeNames";
        public const string ISOLanguages = "ISOLanguages";
        public const string TimeZones = "TimeZones";
        public const string GeoNames = "GeoNames";
        public const string AlternateNamesV2 = "AlternateNamesV2";
    }

    protected GeoNamesOrgDbContext()
    {
    }

    protected GeoNamesOrgDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeoNamesOrgDbContext).Assembly);
    }

    public DbSet<ISOLanguage> ISOLanguages { get; set; } = default!;

    public DbSet<FeatureClass> FeatureClasses { get; set; } = default!;

    public DbSet<FeatureCode> FeatureCodes { get; set; } = default!;

    public DbSet<FeatureCodeName> FeatureCodeNames { get; set; } = default!;

    public DbSet<Continent> Continents { get; set; } = default!;

    public DbSet<Country> Countries { get; set; } = default!;

    public DbSet<TimeZone> TimeZones { get; set; } = default!;

    public DbSet<GeoName> GeoNames { get; set; } = default!;

    public DbSet<AlternateNameV2> AlternateNamesV2 { get; set; } = default!;
}