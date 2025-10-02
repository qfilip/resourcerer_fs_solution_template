using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal sealed class FooConfiguration : IEntityTypeConfiguration<FooRow>
{
    public void Configure(EntityTypeBuilder<FooRow> builder)
    {
        AppDbContext.ConfigureEntity(builder, (e) =>
        {
            e.Property(x => x.Text).IsRequired();
        });
    }
}
