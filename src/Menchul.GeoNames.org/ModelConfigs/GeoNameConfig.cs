using Menchul.GeoNames.org.ModelConfigs.Base;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs
{
    internal sealed class GeoNameConfig : BaseGeoNameIdModelConfig<GeoName>
    {
        public override string TableName => "GeoNames";

        protected override void InternalConfigure(EntityTypeBuilder<GeoName> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200).IsFixedLength(false).IsUnicode();
            builder.Property(x => x.ASCIIName).IsRequired().HasMaxLength(200).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.AlternateNames).IsRequired(false).IsFixedLength(false).IsUnicode();

            builder.Property(x => x.Latitude).IsRequired().HasPrecision(8, 5);
            builder.Property(x => x.Longitude).IsRequired().HasPrecision(8, 5);

            builder.Property(x => x.FeatureClassCode).HasColumnName("FeatureClass").IsRequired(false).HasMaxLength(1).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.FeatureCodeCode).HasColumnName("FeatureCode").IsRequired(false).HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.CountryCode).HasColumnName("CountryCode").IsRequired(false).HasMaxLength(2).IsFixedLength().IsUnicode(false);

            builder.Property(x => x.CountryCodesAlternate).IsRequired(false).HasMaxLength(200).IsFixedLength(false).IsUnicode(false);

            builder.Property(x => x.Admin1Code).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.Admin2Code).IsRequired(false).HasMaxLength(80).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.Admin3Code).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.Admin4Code).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);

            builder.Property(x => x.Population).IsRequired(false);
            builder.Property(x => x.Elevation).IsRequired(false);
            builder.Property(x => x.DEM).IsRequired();
            builder.Property(x => x.TimeZoneName).IsRequired(false).HasMaxLength(30).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.ModificationDate).IsRequired();



            builder
                .HasMany(x => x.AlternateNamesV2)
                .WithOne(x => x.GeoName)
                .HasForeignKey(x => x.GeoNameIdRef)
                .HasPrincipalKey(x => x.GeoNameId)
                .HasConstraintName("FK_AlternateNameV2_GeoNameId__GeoNames_GeoNameId");
        }
    }
}