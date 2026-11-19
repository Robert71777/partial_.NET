using Microsoft.AspNetCore.Http;

namespace RoomHub.Api.Common;

public static class ResultExtensions
{
    /// <summary>
    /// Maps an <see cref="Error"/> to an RFC 9457 ProblemDetails response:
    ///   Validation  → 400
    ///   NotFound    → 404
    ///   Conflict    → 409
    /// </summary>
    public static IResult ToProblem(this Error error) =>
        error.Type switch
        {
            ErrorType.NotFound   => Results.Problem(detail: error.Message, statusCode: 404,
                                        title: error.Code, type: "https://tools.ietf.org/html/rfc9110#section-15.5.5"),
            ErrorType.Conflict   => Results.Problem(detail: error.Message, statusCode: 409,
                                        title: error.Code, type: "https://tools.ietf.org/html/rfc9110#section-15.5.10"),
            _                    => Results.Problem(detail: error.Message, statusCode: 400,
                                        title: error.Code, type: "https://tools.ietf.org/html/rfc9110#section-15.5.1")
        };
}
