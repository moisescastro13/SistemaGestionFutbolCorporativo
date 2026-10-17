using SistemaGestionFutbolCorporativo.Domain.Common;

namespace SistemaGestionFutbolCorporativo.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result) =>
        result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: MapErrorToHttpResult
        );

    public static IResult ToNoContentHttpResult(this Result result) =>
        result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: MapErrorToHttpResult
        );

    public static IResult ToHttpResult<T>(this Result<T> result) =>
        result.Match(
            onSuccess: value => Results.Ok(value),
            onFailure: MapErrorToHttpResult
        );

    public static IResult ToCreatedHttpResult<T>(this Result<T> result, Func<T, string> uriFactory) =>
        result.Match(
            onSuccess: value => Results.Created(uriFactory(value), value),
            onFailure: MapErrorToHttpResult
        );

    private static IResult MapErrorToHttpResult(Error error)
    {
        var extensions = error.Code is not null
            ? new Dictionary<string, object?> { ["errorCode"] = error.Code }
            : null;

        return error.Type switch
        {
            ErrorType.Validation => Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                detail: error.Message,
                extensions: extensions),

            ErrorType.NotFound => Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Not Found",
                detail: error.Message,
                extensions: extensions),

            ErrorType.Conflict => Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                detail: error.Message,
                extensions: extensions),

            _ => Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bad Request",
                detail: error.Message,
                extensions: extensions)
        };
    }
}