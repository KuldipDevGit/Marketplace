using Marketplace.Catalog.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Slug).HasMaxLength(160).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.ImageUrl).HasMaxLength(2048);
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.UpdatedBy).HasMaxLength(256);
        builder.Property(c => c.RowVersion).IsRowVersion();

        builder.HasIndex(c => c.Slug).IsUnique();
        builder.HasIndex(c => c.ParentCategoryId);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
