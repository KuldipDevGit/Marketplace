using Marketplace.Catalog.Api.Contracts;
using Marketplace.Catalog.Application.Categories;
using Marketplace.Catalog.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Catalog.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
public sealed class CategoriesController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<CategoryDto>>> List(
        [FromQuery] Guid? parentId,
        [FromQuery] bool includeInactive = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default) =>
        Ok(await mediator.Send(new ListCategoriesQuery(parentId, includeInactive, page, pageSize, sort), cancellationToken));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(
            new CreateCategoryCommand(request.Name, request.Slug, request.ParentCategoryId, request.Description, request.ImageUrl, request.SortOrder, request.IsActive),
            cancellationToken);
        var category = await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, category);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new UpdateCategoryCommand(id, request.Name, request.ParentCategoryId, request.Description, request.ImageUrl, request.SortOrder, request.IsActive),
            cancellationToken);
        return Ok(await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}
