using Menchul.GeoNames.org.ModelConfigs.Base;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs
{
    internal sealed class ISOLanguageConfig : BaseModelConfig<ISOLanguage>
    {
        public override string TableName => "ISOLanguages";

        protected override void InternalConfigure(EntityTypeBuilder<ISOLanguage> builder)
        {
            builder.HasKey(x => x.ISO639_3).HasName(__primaryKeyName);

            builder.Property(x => x.ISO639_3).IsRequired().HasMaxLength(3).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.ISO639_2).IsRequired(false).HasMaxLength(3).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.ISO639_1).IsRequired(false).HasMaxLength(2).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.Name).IsRequired().IsUnicode();
        }
    }
}