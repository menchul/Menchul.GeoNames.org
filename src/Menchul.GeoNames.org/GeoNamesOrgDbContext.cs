using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/*
 https://learn.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli

dotnet tool install --global dotnet-ef

dotnet tool update --global dotnet-ef

dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet ef

dotnet ef migrations add InitialMigration

dotnet ef migrations remove

dotnet ef database update

 */

namespace Menchul.GeoNames.org
{
    public class GeoNamesOrgDbContext : DbContext
    {
        public const string DBSchema = "gno";

        public GeoNamesOrgDbContext()
        {
        }

        public GeoNamesOrgDbContext(DbContextOptions<GeoNamesOrgDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ISOLanguageBuild(modelBuilder.Entity<ISOLanguage>());
            FeatureClassBuild(modelBuilder.Entity<FeatureClass>());
            FeatureCodeBuild(modelBuilder.Entity<FeatureCode>());
            FeatureCodeNameBuild(modelBuilder.Entity<FeatureCodeName>());
            ContinentBuild(modelBuilder.Entity<Continent>());
            CountryBuild(modelBuilder.Entity<Country>());
            TimeZoneBuild(modelBuilder.Entity<TimeZone>());
            GeoNameBuild(modelBuilder.Entity<GeoName>());
            AlternateNameV2Build(modelBuilder.Entity<AlternateNameV2>());
        }

        #region builders

        private static void ISOLanguageBuild(EntityTypeBuilder<ISOLanguage> entity)
        {
            entity.ToTable("ISOLanguages", DBSchema);
            entity.HasKey(x => x.ISO639_3).HasName("PK_ISOLanguages");

            entity.Property(x => x.ISO639_3).IsRequired().HasMaxLength(3).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.ISO639_2).IsRequired(false).HasMaxLength(3).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.ISO639_1).IsRequired(false).HasMaxLength(2).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.Name).IsRequired().IsUnicode();
        }

        private static void FeatureClassBuild(EntityTypeBuilder<FeatureClass> entity)
        {
            entity.ToTable("FeatureClasses", DBSchema);
            entity.HasKey(x => x.Code).HasName("PK_FeatureClasses");

            entity.Property(x => x.Code).ValueGeneratedNever().IsRequired().HasMaxLength(1).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100).IsFixedLength(false).IsUnicode();

            entity.HasData(ConstData.FeatureClasses);


            entity
                .HasMany(x => x.Codes)
                .WithOne(x => x.FeatureClass)
                .HasForeignKey(x => x.FeatureClassCode)
                .HasPrincipalKey(x => x.Code)
                .HasConstraintName("FK_FeatureCodes_FeatureClassCode__FeatureClass_Code");

            entity
                .HasMany(x => x.GeoNames)
                .WithOne(x => x.FeatureClass)
                .HasForeignKey(x => x.FeatureClassCode)
                .HasPrincipalKey(x => x.Code)
                .HasConstraintName("FK_Geonames_FeatureClass__FeatureClass_Code");
        }

        private static void FeatureCodeBuild(EntityTypeBuilder<FeatureCode> entity)
        {
            entity.ToTable("FeatureCodes", DBSchema);
            entity.HasKey(x => x.Code).HasName("PK_FeatureCodes");

            entity.Property(x => x.Code).ValueGeneratedNever().IsRequired().HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.FeatureClassCode).HasColumnName("FeatureClass").IsRequired().HasMaxLength(1).IsFixedLength().IsUnicode(false);


            entity
                .HasMany(x => x.Names)
                .WithOne(x => x.FeatureCode)
                .HasForeignKey(x => x.FeatureCodeCode)
                .HasPrincipalKey(x => x.Code)
                .HasConstraintName("FK_FeatureCode_Code__FeatureCodeName_FeatureCodeCode");

            entity
                .HasMany(x => x.GeoNames)
                .WithOne(x => x.FeatureCode)
                .HasForeignKey(x => x.FeatureCodeCode)
                .HasPrincipalKey(x => x.Code)
                .HasConstraintName("FK_GeoNames_FeatureCodeCode__FeatureCode_Code");
        }

        private static void FeatureCodeNameBuild(EntityTypeBuilder<FeatureCodeName> entity)
        {
            entity.ToTable("FeatureCodeNames", DBSchema);
            entity.HasKey("FeatureCodeCode", "Language").HasName("PK_FeatureCodeNames");

            entity.Property(x => x.FeatureCodeCode).HasColumnName("FeatureCode").ValueGeneratedNever().IsRequired().HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.Language).ValueGeneratedNever().IsRequired().HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.Name).ValueGeneratedNever().IsRequired().IsFixedLength(false).IsUnicode();
            entity.Property(x => x.Description).ValueGeneratedNever().IsRequired(false).IsFixedLength(false).IsUnicode();
        }

        private static void ContinentBuild(EntityTypeBuilder<Continent> entity)
        {
            entity.ToTable("Continents", DBSchema);
            entity.HasKey(x => x.GeoNameId).HasName("PK_Continents");

            entity.Property(x => x.GeoNameId).ValueGeneratedNever();
            entity.Property(x => x.ISO2).IsRequired().HasMaxLength(2).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100).IsFixedLength(false).IsUnicode();

            entity.HasData(ConstData.Continents);


            entity
                .HasMany(x => x.Countries)
                .WithOne(x => x.Continent)
                .HasForeignKey(x => x.ContinentISO2)
                .HasPrincipalKey(x => x.ISO2)
                .HasConstraintName("FK_Continents_ISO2__Countries_ContinentISO2");
        }

        private static void CountryBuild(EntityTypeBuilder<Country> entity)
        {
            entity.ToTable("Countries", DBSchema);
            entity.HasKey(x => x.GeoNameId).HasName("PK_Countries");

            entity.Property(x => x.GeoNameId).ValueGeneratedNever();
            entity.Property(x => x.ISONumeric).ValueGeneratedNever();
            entity.Property(x => x.ISO2).IsRequired().HasMaxLength(2).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.ISO3).IsRequired().HasMaxLength(3).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.Fips).IsRequired(false).HasMaxLength(2).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.EquivalentFipsCode).IsRequired(false).HasMaxLength(2).IsFixedLength().IsUnicode(false);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(100).IsFixedLength(false).IsUnicode();
            entity.Property(x => x.Capital).IsRequired(false).HasMaxLength(100).IsFixedLength(false).IsUnicode();

            entity.Property(x => x.Area).IsRequired().HasPrecision(10, 2);
            entity.Property(x => x.Population).IsRequired();

            entity.Property(x => x.ContinentISO2).IsRequired().HasMaxLength(2).IsFixedLength().IsUnicode(false);

            entity.Property(x => x.TLD).IsRequired(false).HasMaxLength(3).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.CurrencyCode).IsRequired(false).HasMaxLength(3).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.CurrencyName).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);

            entity.Property(x => x.PhoneCode).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.PostalCodeFormat).IsRequired(false).HasMaxLength(200).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.PostalCodeRegex).IsRequired(false).HasMaxLength(200).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.Languages).IsRequired(false).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.Neighbours).IsRequired(false).IsFixedLength(false).IsUnicode(false);




            entity
                .HasMany(x => x.GeoNames)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryCode)
                .HasPrincipalKey(x => x.ISO2)
                .HasConstraintName("FK_GeoNames_CountryCode__Countries_ISO2");

            entity
                .HasMany(x => x.TimeZones)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryCode)
                .HasPrincipalKey(x => x.ISO2)
                .HasConstraintName("FK_TimeZones_CountryCode__Countries_ISO2");
        }

        private static void TimeZoneBuild(EntityTypeBuilder<TimeZone> entity)
        {
            entity.ToTable("TimeZones", DBSchema);
            entity.HasKey(x => x.Name).HasName("PK_TimeZones");

            entity.Property(x => x.Name).ValueGeneratedNever().IsRequired().HasMaxLength(30).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.CountryCode).IsRequired().HasMaxLength(2).IsFixedLength().IsUnicode(false);

            entity.Property(x => x.GMTOffset).IsRequired().HasPrecision(4, 2);
            entity.Property(x => x.DSTOffset).IsRequired().HasPrecision(4, 2);
            entity.Property(x => x.RawOffset).IsRequired().HasPrecision(4, 2);



            entity
                .HasMany(x => x.GeoNames)
                .WithOne(x => x.TimeZone)
                .HasForeignKey(x => x.TimeZoneName)
                .HasPrincipalKey(x => x.Name)
                .HasConstraintName("FK_GeoNames_TimeZoneName__TimeZones_Name");
        }

        private static void GeoNameBuild(EntityTypeBuilder<GeoName> entity)
        {
            entity.ToTable("GeoNames", DBSchema);
            entity.HasKey(x => x.GeoNameId).HasName("PK_GeoNames");

            entity.Property(x => x.GeoNameId).ValueGeneratedNever();

            entity.Property(x => x.Name).IsRequired().HasMaxLength(200).IsFixedLength(false).IsUnicode();
            entity.Property(x => x.ASCIIName).IsRequired().HasMaxLength(200).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.AlternateNames).IsRequired(false).IsFixedLength(false).IsUnicode();

            entity.Property(x => x.Latitude).IsRequired().HasPrecision(8, 5);
            entity.Property(x => x.Longitude).IsRequired().HasPrecision(8, 5);

            entity.Property(x => x.FeatureClassCode).HasColumnName("FeatureClass").IsRequired(false).HasMaxLength(1).IsFixedLength().IsUnicode(false);
            entity.Property(x => x.FeatureCodeCode).HasColumnName("FeatureCode").IsRequired(false).HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.CountryCode).HasColumnName("CountryCode").IsRequired(false).HasMaxLength(2).IsFixedLength().IsUnicode(false);

            entity.Property(x => x.CountryCodesAlternate).IsRequired(false).HasMaxLength(200).IsFixedLength(false).IsUnicode(false);

            entity.Property(x => x.Admin1Code).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.Admin2Code).IsRequired(false).HasMaxLength(80).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.Admin3Code).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.Admin4Code).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);

            entity.Property(x => x.Population).IsRequired(false);
            entity.Property(x => x.Elevation).IsRequired(false);
            entity.Property(x => x.DEM).IsRequired();
            entity.Property(x => x.TimeZoneName).IsRequired(false).HasMaxLength(30).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.ModificationDate).IsRequired();



            entity
                .HasMany(x => x.AlternateNamesV2)
                .WithOne(x => x.GeoName)
                .HasForeignKey(x => x.GeoNameIdRef)
                .HasPrincipalKey(x => x.GeoNameId)
                .HasConstraintName("FK_AlternateNameV2_GeoNameId__GeoNames_GeoNameId");
        }

        private static void AlternateNameV2Build(EntityTypeBuilder<AlternateNameV2> entity)
        {
            entity.ToTable("AlternateNamesV2", DBSchema);
            entity.HasKey(x => x.GeoNameId).HasName("PK_AlternateNamesV2");

            entity.Property(x => x.GeoNameId).ValueGeneratedNever();

            entity.Property(x => x.GeoNameIdRef);
            entity.Property(x => x.Language).IsRequired(false).HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            entity.Property(x => x.Name).IsRequired().IsFixedLength(false).IsUnicode();
            entity.Property(x => x.C4).IsRequired(false).IsFixedLength(false).IsUnicode();
            entity.Property(x => x.C5).IsRequired(false).IsFixedLength(false).IsUnicode();
            entity.Property(x => x.C6).IsRequired(false).IsFixedLength(false).IsUnicode();
            entity.Property(x => x.C7).IsRequired(false).IsFixedLength(false).IsUnicode();
            entity.Property(x => x.C8).IsRequired(false).IsFixedLength(false).IsUnicode();
            entity.Property(x => x.C9).IsRequired(false).IsFixedLength(false).IsUnicode();
        }

        #endregion builders




        public DbSet<ISOLanguage> ISOLanguages { get; set; }

        public DbSet<FeatureClass> FeatureClasses { get; set; }

        public DbSet<FeatureCode> FeatureCodes { get; set; }

        public DbSet<FeatureCodeName> FeatureCodeNames { get; set; }

        public DbSet<Continent> Continents { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<TimeZone> TimeZones { get; set; }

        public DbSet<GeoName> GeoNames { get; set; }

        public DbSet<AlternateNameV2> AlternateNamesV2 { get; set; }
    }
}