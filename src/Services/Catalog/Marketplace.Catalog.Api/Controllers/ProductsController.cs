using Marketplace.Catalog.Api.Contracts;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Application.Products;
using Marketplace.Catalog.Domain.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Catalog.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductsController(ISender mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<ProductSummaryDto>>> List(
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? sellerId,
        [FromQuery] Guid? brandId,
        [FromQuery] ProductStatus? status,
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default) =>
        Ok(await mediator.Send(new ListProductsQuery(categoryId, sellerId, brandId, status, q, page, pageSize, sort), cancellationToken));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new GetProductByIdQuery(id), cancellationToken));

    [HttpGet("by-slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetBySlug(string slug, CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new GetProductBySlugQuery(slug), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Seller")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.CategoryId,
            request.BrandId,
            request.Name,
            request.Slug,
            request.Description,
            request.Sku,
            (request.Attributes ?? []).Select(a => new ProductAttributeDto(a.Name, a.Value)).ToList(),
            (request.Images ?? []).Select(i => new NewProductImage(i.Url, i.AltText, i.SortOrder, i.IsPrimary)).ToList());

        var id = await mediator.Send(command, cancellationToken);
        var product = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, product);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.CategoryId,
            request.BrandId,
            request.Name,
            request.Description,
            request.Sku,
            (request.Attributes ?? []).Select(a => new ProductAttributeDto(a.Name, a.Value)).ToList(),
            Convert.FromBase64String(request.RowVersion));

        await mediator.Send(command, cancellationToken);
        return Ok(await mediator.Send(new GetProductByIdQuery(id), cancellationToken));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<ActionResult<ProductDto>> UpdateStatus(Guid id, [FromBody] UpdateProductStatusRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateProductStatusCommand(id, request.Status, Convert.FromBase64String(request.RowVersion)), cancellationToken);
        return Ok(await mediator.Send(new GetProductByIdQuery(id), cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/images")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> AddImage(Guid id, [FromBody] AddProductImageRequest request, CancellationToken cancellationToken)
    {
        var imageId = await mediator.Send(new AddProductImageCommand(id, request.Url, request.AltText, request.SortOrder, request.IsPrimary), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { imageId });
    }

    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> RemoveImage(Guid id, Guid imageId, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveProductImageCommand(id, imageId), cancellationToken);
        return NoContent();
    }
}
