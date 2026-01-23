using Menchul.GeoNames.org.ModelConfigs.Base;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs
{
    internal sealed class FeatureCodeNameConfig : BaseModelConfig<FeatureCodeName>
    {
        public override string TableName => "FeatureCodeNames";

        protected override void InternalConfigure(EntityTypeBuilder<FeatureCodeName> builder)
        {
            builder.HasKey("FeatureCodeCode", "Language").HasName(__primaryKeyName);

            builder.Property(x => x.FeatureCodeCode).IsRequired().HasColumnName("FeatureCode").ValueGeneratedNever().HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.Language).IsRequired().ValueGeneratedNever().HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.Name).IsRequired().ValueGeneratedNever().IsFixedLength(false).IsUnicode();
            builder.Property(x => x.Description).IsRequired(false).ValueGeneratedNever().IsFixedLength(false).IsUnicode();
        }
    }
}