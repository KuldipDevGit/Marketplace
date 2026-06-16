using Marketplace.Contracts;

namespace Marketplace.Application.Abstractions;

/// <summary>
/// Publishes an integration event to the bus. The infrastructure implementation stages it in the
/// transactional outbox so it is delivered atomically with the unit of work (ADR-0008, DB-9, BE-10).
/// </summary>
public interface IIntegrationEventPublisher
{
    Task PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
