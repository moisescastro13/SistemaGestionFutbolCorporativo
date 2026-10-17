using MediatR;
using Microsoft.Extensions.Logging;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Events;

namespace SistemaGestionFutbolCorporativo.Application.Equipos.Events;

public sealed class TeamCreatedDomainEventHandler(
    ILogger<TeamCreatedDomainEventHandler> logger) : INotificationHandler<DomainEventNotification<TeamCreatedDomainEvent>>
{
    public Task Handle(DomainEventNotification<TeamCreatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var @event = notification.DomainEvent;

        logger.LogInformation(
            "Trazabilidad Dominio: Evento {EventName} procesado [EventId: {EventId}] - Equipo '{Nombre}' registrado con ID {EquipoId} a las {FechaUtc}",
            nameof(TeamCreatedDomainEvent),
            @event.EventId,
            @event.Nombre,
            @event.EquipoId,
            @event.OccurredOnUtc);

        return Task.CompletedTask;
    }
}
