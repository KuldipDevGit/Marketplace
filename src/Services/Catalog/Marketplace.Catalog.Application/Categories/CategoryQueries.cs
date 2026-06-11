using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using MediatR;

namespace Marketplace.Catalog.Application.Categories;

public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;

public sealed class GetCategoryByIdHandler(ICategoryRepository repository)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken) =>
        await repository.GetDtoByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"Category '{request.Id}' was not found.");
}

public sealed record ListCategoriesQuery(Guid? ParentId, bool IncludeInactive, int Page, int PageSize, string? Sort)
    : IRequest<PagedResult<CategoryDto>>;

public sealed class ListCategoriesHandler(ICategoryRepository repository)
    : IRequestHandler<ListCategoriesQuery, PagedResult<CategoryDto>>
{
    public Task<PagedResult<CategoryDto>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken) =>
        repository.ListAsync(request.ParentId, request.IncludeInactive, request.Page, request.PageSize, request.Sort, cancellationToken);
}
