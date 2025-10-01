using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.Configurations;

internal sealed class FooConfiguration : IEntityTypeConfiguration<Foo>
{
    public void Configure(EntityTypeBuilder<Foo> builder)
    {
        AppDbContext.ConfigureEntity(builder, (e) =>
        {
            e.Property(x => x.Text).IsRequired();
        });
    }
}
