using Marketplace.Catalog.Domain.Brands;
using Marketplace.Catalog.Domain.Categories;
using Marketplace.Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Catalog.Infrastructure.Persistence;

/// <summary>
/// Seeds a small, deterministic demo catalog for local development so the database and the API are
/// not empty. Development convenience only — never wired in production. Idempotent: it is a no-op
/// once the catalog has any category row, so it is safe to run on every startup. Fixed identifiers
/// keep the data stable across runs and let products reference their categories and brands.
/// </summary>
public static class CatalogDbSeeder
{
    public static async Task SeedDevelopmentDataAsync(CatalogDbContext db, CancellationToken cancellationToken = default)
    {
        // Ignore the soft-delete filter so a previously-seeded-then-deleted row still counts; we must
        // never re-insert the fixed keys below.
        if (await db.Categories.IgnoreQueryFilters().AnyAsync(cancellationToken))
        {
            return;
        }

        var electronicsId = new Guid("a0000000-0000-0000-0000-000000000001");
        var booksId = new Guid("a0000000-0000-0000-0000-000000000002");
        var homeKitchenId = new Guid("a0000000-0000-0000-0000-000000000003");
        var laptopsId = new Guid("a0000000-0000-0000-0000-000000000004");

        db.Categories.AddRange(
            new Category(electronicsId, "Electronics", "electronics", null, "Phones, audio, computing and accessories.", null, 1, isActive: true),
            new Category(booksId, "Books", "books", null, "Fiction, non-fiction and technical titles.", null, 2, isActive: true),
            new Category(homeKitchenId, "Home & Kitchen", "home-kitchen", null, "Cookware, appliances and home essentials.", null, 3, isActive: true),
            new Category(laptopsId, "Laptops", "laptops", electronicsId, "Notebooks and ultrabooks.", null, 1, isActive: true));

        var acmeId = new Guid("b0000000-0000-0000-0000-000000000001");
        var globexId = new Guid("b0000000-0000-0000-0000-000000000002");

        db.Brands.AddRange(
            new Brand(acmeId, "Acme", "acme", null, isActive: true),
            new Brand(globexId, "Globex", "globex", null, isActive: true));

        // No Seller service in this build — a fixed placeholder owner for the demo products.
        var sellerId = new Guid("c0000000-0000-0000-0000-000000000001");

        db.Products.AddRange(
            BuildProduct(
                new Guid("d0000000-0000-0000-0000-000000000001"), sellerId, laptopsId, acmeId,
                "Acme UltraBook 14", "acme-ultrabook-14",
                "A 14-inch ultrabook with an aluminium chassis and all-day battery life.", "ACME-UB14",
                new Guid("e0000000-0000-0000-0000-000000000001"),
                [new ProductAttribute("Color", "Silver"), new ProductAttribute("RAM", "16 GB")]),
            BuildProduct(
                new Guid("d0000000-0000-0000-0000-000000000002"), sellerId, electronicsId, globexId,
                "Globex Smart Speaker", "globex-smart-speaker",
                "Voice-controlled smart speaker with room-filling sound.", "GLX-SS1",
                new Guid("e0000000-0000-0000-0000-000000000002"),
                [new ProductAttribute("Color", "Charcoal")]),
            BuildProduct(
                new Guid("d0000000-0000-0000-0000-000000000003"), sellerId, booksId, null,
                "The Pragmatic Programmer", "the-pragmatic-programmer",
                "Your journey to mastery, 20th anniversary edition.", "BK-PP-001",
                new Guid("e0000000-0000-0000-0000-000000000003"),
                [new ProductAttribute("Format", "Hardcover")]),
            BuildProduct(
                new Guid("d0000000-0000-0000-0000-000000000004"), sellerId, homeKitchenId, acmeId,
                "Acme Chef's Knife 8\"", "acme-chefs-knife-8",
                "Full-tang 8-inch chef's knife in high-carbon stainless steel.", "ACME-CK8",
                new Guid("e0000000-0000-0000-0000-000000000004"),
                [new ProductAttribute("Material", "Stainless Steel"), new ProductAttribute("Length", "8 in")]));

        await db.SaveChangesAsync(cancellationToken);
    }

    private static Product BuildProduct(
        Guid id,
        Guid sellerId,
        Guid categoryId,
        Guid? brandId,
        string name,
        string slug,
        string description,
        string sku,
        Guid imageId,
        IReadOnlyList<ProductAttribute> attributes)
    {
        var product = new Product(id, sellerId, categoryId, brandId, name, slug, description, sku);
        product.SetAttributes(attributes);
        product.AddImage(imageId, $"https://picsum.photos/seed/{slug}/600/400", name, 0, isPrimary: true);
        product.ChangeStatus(ProductStatus.Active);
        return product;
    }
}
