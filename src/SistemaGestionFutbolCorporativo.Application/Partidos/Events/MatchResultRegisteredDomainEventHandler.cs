using MediatR;
using Microsoft.Extensions.Logging;
using SistemaGestionFutbolCorporativo.Application.Common;
using SistemaGestionFutbolCorporativo.Domain.Events;

namespace SistemaGestionFutbolCorporativo.Application.Partidos.Events;

public sealed class MatchResultRegisteredDomainEventHandler(
    ILogger<MatchResultRegisteredDomainEventHandler> logger) : INotificationHandler<DomainEventNotification<MatchResultRegisteredDomainEvent>>
{
    public Task Handle(DomainEventNotification<MatchResultRegisteredDomainEvent> notification, CancellationToken cancellationToken)
    {
        var @event = notification.DomainEvent;

        logger.LogInformation(
            "Trazabilidad Dominio: Evento {EventName} procesado [EventId: {EventId}] - Partido {PartidoId} finalizó: Local ({EquipoLocalId}) {GolesLocal} - {GolesVisitante} Visitante ({EquipoVisitanteId}) a las {FechaUtc}",
            nameof(MatchResultRegisteredDomainEvent),
            @event.EventId,
            @event.PartidoId,
            @event.EquipoLocalId,
            @event.GolesLocal,
            @event.GolesVisitante,
            @event.EquipoVisitanteId,
            @event.OccurredOnUtc);

        return Task.CompletedTask;
    }
}
