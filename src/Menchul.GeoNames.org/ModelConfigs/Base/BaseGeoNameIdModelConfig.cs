using Menchul.GeoNames.org.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs.Base
{
    internal abstract class BaseGeoNameIdModelConfig<TCode>
        : BaseModelConfig<TCode> where TCode : BaseGeoNameIdEntity
    {
        public override void Configure(EntityTypeBuilder<TCode> builder)
        {
            builder.HasKey(x => x.GeoNameId).HasName(__primaryKeyName);

            builder.Property(x => x.GeoNameId).ValueGeneratedNever().IsRequired();

            base.Configure(builder);
        }
    }
}