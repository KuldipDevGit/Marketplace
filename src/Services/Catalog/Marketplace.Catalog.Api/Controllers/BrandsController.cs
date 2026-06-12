using Marketplace.Catalog.Api.Contracts;
using Marketplace.Catalog.Application.Brands;
using Marketplace.Catalog.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Catalog.Api.Controllers;

[ApiController]
[Route("api/v1/brands")]
public sealed class BrandsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<BrandDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default) =>
        Ok(await mediator.Send(new ListBrandsQuery(page, pageSize, sort), cancellationToken));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<BrandDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new GetBrandByIdQuery(id), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandDto>> Create([FromBody] CreateBrandRequest request, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(new CreateBrandCommand(request.Name, request.Slug, request.LogoUrl, request.IsActive), cancellationToken);
        var brand = await mediator.Send(new GetBrandByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, brand);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BrandDto>> Update(Guid id, [FromBody] UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateBrandCommand(id, request.Name, request.LogoUrl, request.IsActive), cancellationToken);
        return Ok(await mediator.Send(new GetBrandByIdQuery(id), cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteBrandCommand(id), cancellationToken);
        return NoContent();
    }
}
