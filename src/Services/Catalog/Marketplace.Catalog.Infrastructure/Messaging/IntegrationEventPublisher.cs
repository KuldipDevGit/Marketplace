using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Contracts;
using MassTransit;

namespace Marketplace.Catalog.Infrastructure.Messaging;

/// <summary>
/// Publishes integration events through MassTransit. Because the bus is configured with the EF
/// transactional outbox, the publish is staged and delivered atomically with the unit of work
/// (ADR-0008, DB-9). Published by concrete type so consumers receive the derived event.
/// </summary>
internal sealed class IntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public Task PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default) =>
        publishEndpoint.Publish(integrationEvent, integrationEvent.GetType(), cancellationToken);
}
