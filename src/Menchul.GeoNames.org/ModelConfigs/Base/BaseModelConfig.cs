using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menchul.GeoNames.org.ModelConfigs.Base
{
    internal abstract class BaseModelConfig<T>
        : IEntityTypeConfiguration<T> where T : class
    {
        public virtual string? Schema => GeoNamesOrgDbContext.DBSchema;

        public abstract string TableName { get; }

        public string __primaryKeyName => "PK_" + TableName;

        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.ToTable(TableName, Schema);

            InternalConfigure(builder);
        }

        protected abstract void InternalConfigure(EntityTypeBuilder<T> builder);
    }
}