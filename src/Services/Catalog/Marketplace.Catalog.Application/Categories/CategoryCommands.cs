using FluentValidation;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Categories;
using Marketplace.Contracts.Catalog.V1;
using MediatR;

namespace Marketplace.Catalog.Application.Categories;

// ----- Create -----

public sealed record CreateCategoryCommand(
    string Name,
    string? Slug,
    Guid? ParentCategoryId,
    string? Description,
    string? ImageUrl,
    int SortOrder,
    bool IsActive) : IRequest<Guid>;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Slug).MaximumLength(160);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}

public sealed class CreateCategoryHandler(
    ICategoryRepository repository,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    TimeProvider clock) : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var slug = Slug.From(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);
        if (await repository.SlugExistsAsync(slug, null, cancellationToken))
        {
            throw new ConflictException($"A category with slug '{slug}' already exists.");
        }

        if (request.ParentCategoryId is { } parentId && !await repository.ExistsAsync(parentId, cancellationToken))
        {
            throw new NotFoundException($"Parent category '{parentId}' was not found.");
        }

        var category = new Category(
            Guid.NewGuid(), request.Name, slug, request.ParentCategoryId,
            request.Description, request.ImageUrl, request.SortOrder, request.IsActive);

        await repository.AddAsync(category, cancellationToken);
        await publisher.PublishAsync(
            new CategoryCreatedIntegrationEvent
            {
                CategoryId = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ParentCategoryId = category.ParentCategoryId,
                OccurredOnUtc = clock.GetUtcNow().UtcDateTime,
            },
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}

// ----- Update -----

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    Guid? ParentCategoryId,
    string? Description,
    string? ImageUrl,
    int SortOrder,
    bool IsActive) : IRequest;

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}

public sealed class UpdateCategoryHandler(
    ICategoryRepository repository,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    TimeProvider clock) : IRequestHandler<UpdateCategoryCommand>
{
    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Category '{request.Id}' was not found.");

        if (request.ParentCategoryId is { } parentId
            && parentId != category.ParentCategoryId
            && !await repository.ExistsAsync(parentId, cancellationToken))
        {
            throw new NotFoundException($"Parent category '{parentId}' was not found.");
        }

        category.Update(request.Name, request.ParentCategoryId, request.Description, request.ImageUrl, request.SortOrder, request.IsActive);
        await publisher.PublishAsync(
            new CategoryUpdatedIntegrationEvent
            {
                CategoryId = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                OccurredOnUtc = clock.GetUtcNow().UtcDateTime,
            },
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

// ----- Delete -----

public sealed record DeleteCategoryCommand(Guid Id) : IRequest;

public sealed class DeleteCategoryHandler(ICategoryRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Category '{request.Id}' was not found.");

        category.MarkDeleted();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
