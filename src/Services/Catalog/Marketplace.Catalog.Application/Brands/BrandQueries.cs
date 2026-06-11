using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using MediatR;

namespace Marketplace.Catalog.Application.Brands;

public sealed record GetBrandByIdQuery(Guid Id) : IRequest<BrandDto>;

public sealed class GetBrandByIdHandler(IBrandRepository repository) : IRequestHandler<GetBrandByIdQuery, BrandDto>
{
    public async Task<BrandDto> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken) =>
        await repository.GetDtoByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Brand '{request.Id}' was not found.");
}

public sealed record ListBrandsQuery(int Page, int PageSize, string? Sort) : IRequest<PagedResult<BrandDto>>;

public sealed class ListBrandsHandler(IBrandRepository repository) : IRequestHandler<ListBrandsQuery, PagedResult<BrandDto>>
{
    public Task<PagedResult<BrandDto>> Handle(ListBrandsQuery request, CancellationToken cancellationToken) =>
        repository.ListAsync(request.Page, request.PageSize, request.Sort, cancellationToken);
}
