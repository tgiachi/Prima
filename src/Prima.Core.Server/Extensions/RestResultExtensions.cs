using System.Net;
using Microsoft.AspNetCore.Http;
using Prima.Core.Server.Data.Rest.Base;

namespace Prima.Core.Server.Extensions;

/// <summary>
/// Extension methods for working with RestResultObject in Minimal APIs
/// </summary>
public static class RestResultExtensions
{
    /// <summary>
    /// Converts a RestResultObject to an appropriate IResult for Minimal APIs
    /// </summary>
    public static IResult ToResult<T>(this RestResultObject<T> restResult)
    {
        return Results.Json(restResult, statusCode: (int)restResult.StatusCode);
    }

    /// <summary>
    /// Extension method to create success result
    /// </summary>
    public static IResult Success<T>(this IResultExtensions _, T data, string message = "Operation completed successfully")
    {
        var result = RestResultObject<T>.CreateSuccess(data, message);
        return result.ToResult();
    }

    /// <summary>
    /// Extension method to create error result
    /// </summary>
    public static IResult Error<T>(
        this IResultExtensions _, string message,
        HttpStatusCode statusCode = System.Net.HttpStatusCode.BadRequest, List<string> errors = null
    )
    {
        var result = RestResultObject<T>.CreateError(message, statusCode, errors);
        return result.ToResult();
    }
}
