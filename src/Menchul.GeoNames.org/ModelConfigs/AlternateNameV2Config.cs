using Menchul.GeoNames.org.ModelConfigs.Base;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs
{
    internal sealed class AlternateNameV2Config : BaseGeoNameIdModelConfig<AlternateNameV2>
    {
        public override string TableName => "AlternateNamesV2";

        protected override void InternalConfigure(EntityTypeBuilder<AlternateNameV2> builder)
        {
            builder.Property(x => x.GeoNameIdRef);
            builder.Property(x => x.Language).IsRequired(false).HasMaxLength(10).IsFixedLength(false).IsUnicode(false);
            builder.Property(x => x.Name).IsRequired().IsFixedLength(false).IsUnicode();
            builder.Property(x => x.C4).IsRequired(false).IsFixedLength(false).IsUnicode();
            builder.Property(x => x.C5).IsRequired(false).IsFixedLength(false).IsUnicode();
            builder.Property(x => x.C6).IsRequired(false).IsFixedLength(false).IsUnicode();
            builder.Property(x => x.C7).IsRequired(false).IsFixedLength(false).IsUnicode();
            builder.Property(x => x.C8).IsRequired(false).IsFixedLength(false).IsUnicode();
            builder.Property(x => x.C9).IsRequired(false).IsFixedLength(false).IsUnicode();
        }
    }
}