using Marketplace.Catalog.Domain.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name).HasMaxLength(150).IsRequired();
        builder.Property(b => b.Slug).HasMaxLength(160).IsRequired();
        builder.Property(b => b.LogoUrl).HasMaxLength(2048);
        builder.Property(b => b.CreatedBy).HasMaxLength(256);
        builder.Property(b => b.UpdatedBy).HasMaxLength(256);
        builder.Property(b => b.RowVersion).IsRowVersion();

        builder.HasIndex(b => b.Slug).IsUnique();

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
