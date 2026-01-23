using Menchul.GeoNames.org.ModelConfigs.Base;
using Menchul.GeoNames.org.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs
{
    internal sealed class ContinentConfig : BaseGeoNameIdModelConfig<Continent>
    {
        public override string TableName => "Continents";

        protected override void InternalConfigure(EntityTypeBuilder<Continent> builder)
        {
            builder.Property(x => x.ISO2).IsRequired().HasMaxLength(2).IsFixedLength().IsUnicode(false);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100).IsFixedLength(false).IsUnicode();

            builder.HasData(ConstData.Continents);


            builder
                .HasMany(x => x.Countries)
                .WithOne(x => x.Continent)
                .HasForeignKey(x => x.ContinentISO2)
                .HasPrincipalKey(x => x.ISO2)
                .HasConstraintName("FK_Continents_ISO2__Countries_ContinentISO2");
        }
    }
}