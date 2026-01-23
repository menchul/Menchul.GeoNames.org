using Menchul.GeoNames.org.ModelConfigs.Base;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs
{
    internal sealed class FeatureCodeConfig : BaseModelConfig<FeatureCode>
    {
        public override string TableName => "FeatureCodes";

        protected override void InternalConfigure(EntityTypeBuilder<FeatureCode> builder)
        {
            builder.HasKey(x => x.Code).HasName(__primaryKeyName);

            builder.Property(x => x.Code).IsRequired().ValueGeneratedNever().HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.FeatureClassCode).IsRequired().HasColumnName("FeatureClass").HasMaxLength(1).IsFixedLength().IsUnicode(false);


            builder
                .HasMany(x => x.Names)
                .WithOne(x => x.FeatureCode)
                .HasForeignKey(x => x.FeatureCodeCode)
                .HasPrincipalKey(x => x.Code)
                .HasConstraintName("FK_FeatureCode_Code__FeatureCodeName_FeatureCodeCode");

            builder
                .HasMany(x => x.GeoNames)
                .WithOne(x => x.FeatureCode)
                .HasForeignKey(x => x.FeatureCodeCode)
                .HasPrincipalKey(x => x.Code)
                .HasConstraintName("FK_GeoNames_FeatureCodeCode__FeatureCode_Code");
        }
    }
}