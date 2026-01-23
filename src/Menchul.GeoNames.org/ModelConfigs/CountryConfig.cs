using Menchul.GeoNames.org.ModelConfigs.Base;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs
{
    internal sealed class CountryConfig : BaseGeoNameIdModelConfig<Country>
    {
        public override string TableName => "Countries";

        protected override void InternalConfigure(EntityTypeBuilder<Country> builder)
        {
            builder.Property(x => x.ISONumeric).ValueGeneratedNever();
            builder.Property(x => x.ISO2).IsRequired().HasMaxLength(2).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.ISO3).IsRequired().HasMaxLength(3).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.Fips).IsRequired(false).HasMaxLength(2).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.EquivalentFipsCode).IsRequired(false).HasMaxLength(2).IsFixedLength().IsUnicode(false);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100).IsFixedLength(false).IsUnicode();
            builder.Property(x => x.Capital).IsRequired(false).HasMaxLength(100).IsFixedLength(false).IsUnicode();

            builder.Property(x => x.Area).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.Population).IsRequired();

            builder.Property(x => x.ContinentISO2).IsRequired().HasMaxLength(2).IsFixedLength().IsUnicode(false);

            builder.Property(x => x.TLD).IsRequired(false).HasMaxLength(3).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.CurrencyCode).IsRequired(false).HasMaxLength(3).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.CurrencyName).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);

            builder.Property(x => x.PhoneCode).IsRequired(false).HasMaxLength(20).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.PostalCodeFormat).IsRequired(false).HasMaxLength(200).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.PostalCodeRegex).IsRequired(false).HasMaxLength(200).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.Languages).IsRequired(false).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.Neighbours).IsRequired(false).IsFixedLength(false).IsUnicode(false);



            builder
                .HasMany(x => x.GeoNames)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryCode)
                .HasPrincipalKey(x => x.ISO2)
                .HasConstraintName("FK_GeoNames_CountryCode__Countries_ISO2");

            builder
                .HasMany(x => x.TimeZones)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryCode)
                .HasPrincipalKey(x => x.ISO2)
                .HasConstraintName("FK_TimeZones_CountryCode__Countries_ISO2");
        }
    }
}