using FluentValidation;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Brands;
using MediatR;

namespace Marketplace.Catalog.Application.Brands;

// ----- Create -----

public sealed record CreateBrandCommand(string Name, string? Slug, string? LogoUrl, bool IsActive) : IRequest<Guid>;

public sealed class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Slug).MaximumLength(160);
    }
}

public sealed class CreateBrandHandler(IBrandRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateBrandCommand, Guid>
{
    public async Task<Guid> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var slug = Slug.From(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);
        if (await repository.SlugExistsAsync(slug, null, cancellationToken))
        {
            throw new ConflictException($"A brand with slug '{slug}' already exists.");
        }

        var brand = new Brand(Guid.NewGuid(), request.Name, slug, request.LogoUrl, request.IsActive);
        await repository.AddAsync(brand, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return brand.Id;
    }
}

// ----- Update -----

public sealed record UpdateBrandCommand(Guid Id, string Name, string? LogoUrl, bool IsActive) : IRequest;

public sealed class UpdateBrandValidator : AbstractValidator<UpdateBrandCommand>
{
    public UpdateBrandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}

public sealed class UpdateBrandHandler(IBrandRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateBrandCommand>
{
    public async Task Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Brand '{request.Id}' was not found.");

        brand.Update(request.Name, request.LogoUrl, request.IsActive);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

// ----- Delete -----

public sealed record DeleteBrandCommand(Guid Id) : IRequest;

public sealed class DeleteBrandHandler(IBrandRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteBrandCommand>
{
    public async Task Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Brand '{request.Id}' was not found.");

        brand.MarkDeleted();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
