using System.Net;
using FluentAssertions;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Application.Products;
using Marketplace.Catalog.Domain.Products;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Marketplace.Catalog.FunctionalTests;

/// <summary>
/// Verifies the list endpoints' [FromQuery] parameter objects bind the query string correctly (the
/// names are pinned, so e.g. ?page=3 must reach the filter as Page=3). Uses a stub repository that
/// records the filter, so no database is involved.
/// </summary>
public sealed class ListQueryBindingTests
{
    [Fact]
    public async Task Product_list_query_parameters_bind_to_the_filter()
    {
        var repository = new CapturingProductRepository();
        using var factory = new CatalogApiFactory().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services => services.AddScoped<IProductRepository>(_ => repository)));
        var client = factory.CreateClient();
        var categoryId = Guid.NewGuid();

        var response = await client.GetAsync(
            $"/api/v1/products?categoryId={categoryId}&page=3&pageSize=7&sort=name&q=widget");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        repository.LastFilter.Should().NotBeNull();
        repository.LastFilter!.CategoryId.Should().Be(categoryId);
        repository.LastFilter.Page.Should().Be(3);
        repository.LastFilter.PageSize.Should().Be(7);
        repository.LastFilter.Sort.Should().Be("name");
        repository.LastFilter.Query.Should().Be("widget");
    }

    private sealed class CapturingProductRepository : IProductRepository
    {
        public ProductListFilter? LastFilter { get; private set; }

        public Task<PagedResult<ProductSummaryDto>> ListAsync(ProductListFilter filter, CancellationToken cancellationToken = default)
        {
            LastFilter = filter;
            return Task.FromResult(new PagedResult<ProductSummaryDto>([], filter.Page, filter.PageSize, 0));
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> SlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task AddAsync(Product product, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void SetOriginalRowVersion(Product product, byte[] rowVersion) => throw new NotSupportedException();
        public Task<ProductDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<ProductDto?> GetDtoBySlugAsync(string slug, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
