using Menchul.GeoNames.org.ModelConfigs.Base;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs
{
    internal sealed class FeatureClassConfig : BaseModelConfig<FeatureClass>
    {
        public override string TableName => "FeatureClasses";

        protected override void InternalConfigure(EntityTypeBuilder<FeatureClass> builder)
        {
            builder.HasKey(x => x.Code).HasName(__primaryKeyName);

            builder.Property(x => x.Code).IsRequired().ValueGeneratedNever().HasMaxLength(1).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100).IsFixedLength(false).IsUnicode();

            builder.HasData(ConstData.FeatureClasses);


            builder
                .HasMany(x => x.Codes)
                .WithOne(x => x.FeatureClass)
                .HasForeignKey(x => x.FeatureClassCode)
                .HasPrincipalKey(x => x.Code)
                .HasConstraintName("FK_FeatureCodes_FeatureClassCode__FeatureClass_Code");

            builder
                .HasMany(x => x.GeoNames)
                .WithOne(x => x.FeatureClass)
                .HasForeignKey(x => x.FeatureClassCode)
                .HasPrincipalKey(x => x.Code)
                .HasConstraintName("FK_Geonames_FeatureClass__FeatureClass_Code");
        }
    }
}