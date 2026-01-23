using Menchul.GeoNames.org.ModelConfigs.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeZone = Menchul.GeoNames.org.Models.TimeZone;

namespace Menchul.GeoNames.org.ModelConfigs
{
    internal sealed class TimeZoneConfig : BaseModelConfig<TimeZone>
    {
        public override string TableName => "TimeZones";

        protected override void InternalConfigure(EntityTypeBuilder<TimeZone> builder)
        {
            builder.HasKey(x => x.Name).HasName(__primaryKeyName);

            builder.Property(x => x.Name).IsRequired().ValueGeneratedNever().HasMaxLength(30).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.CountryCode).IsRequired().HasMaxLength(2).IsFixedLength().IsUnicode(false);

            builder.Property(x => x.GMTOffset).IsRequired().HasPrecision(4, 2);
            builder.Property(x => x.DSTOffset).IsRequired().HasPrecision(4, 2);
            builder.Property(x => x.RawOffset).IsRequired().HasPrecision(4, 2);


            builder
                .HasMany(x => x.GeoNames)
                .WithOne(x => x.TimeZone)
                .HasForeignKey(x => x.TimeZoneName)
                .HasPrincipalKey(x => x.Name)
                .HasConstraintName("FK_GeoNames_TimeZoneName__TimeZones_Name");
        }
    }
}