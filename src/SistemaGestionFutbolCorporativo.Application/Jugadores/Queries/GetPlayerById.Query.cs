using MediatR;
using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Application.Jugadores.Queries;

public sealed record GetPlayerByIdQuery(Guid Id) : IRequest<Result<PlayerDetailResponse>>;

public sealed class GetPlayerByIdQueryHandler(IPlayerQueries playerQueries)
    : IRequestHandler<GetPlayerByIdQuery, Result<PlayerDetailResponse>>
{
    public async Task<Result<PlayerDetailResponse>> Handle(
        GetPlayerByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var player = await playerQueries.GetByIdAsync(request.Id, cancellationToken);
        if (player is null)
        {
            return Error.NotFound(
                $"No se encontró ningún jugador con el ID '{request.Id}'.", 
                "Jugador.NotFound");
        }

        return player;
    }
}
