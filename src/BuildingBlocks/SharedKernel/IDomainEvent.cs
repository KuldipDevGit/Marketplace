namespace Marketplace.SharedKernel;

/// <summary>
/// Marker for an event raised by an aggregate when its state changes. Domain events are
/// in-process only and never cross a service boundary — they are translated to integration
/// events at the Application boundary (BE-10).
/// </summary>
public interface IDomainEvent;
