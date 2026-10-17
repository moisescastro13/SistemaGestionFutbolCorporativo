using MediatR;
using Microsoft.Extensions.Logging;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Infrastructure.Persistence;

public sealed class DomainEventDispatcher(
    IPublisher publisher,
    ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public async Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            logger.LogInformation(
                "Despachando evento de dominio: {EventType} [EventId: {EventId}]",
                domainEvent.GetType().Name,
                domainEvent.EventId);

            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            if (Activator.CreateInstance(notificationType, domainEvent) is INotification notification)
            {
                await publisher.Publish(notification, cancellationToken);
            }
        }
    }
}
