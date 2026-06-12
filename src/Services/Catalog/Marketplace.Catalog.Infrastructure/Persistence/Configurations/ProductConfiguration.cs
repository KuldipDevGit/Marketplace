using Marketplace.Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(250).IsRequired();
        builder.Property(p => p.Slug).HasMaxLength(260).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(4000).IsRequired();
        builder.Property(p => p.Sku).HasMaxLength(64).IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.CreatedBy).HasMaxLength(256);
        builder.Property(p => p.UpdatedBy).HasMaxLength(256);
        builder.Property(p => p.RowVersion).IsRowVersion();

        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.SellerId);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.BrandId);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.OwnsMany(p => p.Images, image =>
        {
            image.ToTable("ProductImages");
            image.WithOwner().HasForeignKey("ProductId");
            image.HasKey(i => i.Id);
            image.Property(i => i.Url).HasMaxLength(2048).IsRequired();
            image.Property(i => i.AltText).HasMaxLength(250);
        });

        builder.OwnsMany(p => p.Attributes, attribute =>
        {
            attribute.ToTable("ProductAttributes");
            attribute.WithOwner().HasForeignKey("ProductId");
            attribute.Property(a => a.Name).HasMaxLength(100).IsRequired();
            attribute.Property(a => a.Value).HasMaxLength(500).IsRequired();
        });
    }
}
