using System.Net;
using System.Text.Json.Serialization;

namespace Prima.Core.Server.Data.Rest.Base;

/// <summary>
/// Generic class for standardized REST API responses
/// </summary>
/// <typeparam name="T">The type of data returned</typeparam>
public class RestResultObject<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// HTTP status code
    /// </summary>
    [JsonIgnore] // This property will not be serialized in the response
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Status code as integer for serialization in the response
    /// </summary>
    [JsonIgnore]
    public int Status => (int)StatusCode;

    /// <summary>
    /// Descriptive message about the operation
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Data returned by the operation
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Any errors that occurred during the operation
    /// </summary>
    public List<string> Errors { get; set; } = null;

    /// <summary>
    /// Timestamp of the operation
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public RestResultObject()
    {
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Creates a success result
    /// </summary>
    /// <param name="data">The data to return</param>
    /// <param name="message">Optional message</param>
    /// <param name="statusCode">HTTP status code (default is OK)</param>
    /// <returns>A RestResultObject with Success = true</returns>
    public static RestResultObject<T> CreateSuccess(
        T data, string message = "Operation completed successfully", HttpStatusCode statusCode = HttpStatusCode.OK
    )
    {
        return new RestResultObject<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    /// <summary>
    /// Creates a success result with no data (for void operations)
    /// </summary>
    /// <param name="message">Optional message</param>
    /// <param name="statusCode">HTTP status code (default is OK)</param>
    /// <returns>A RestResultObject with Success = true and default data</returns>
    public static RestResultObject<T> CreateSuccess(
        string message = "Operation completed successfully", HttpStatusCode statusCode = HttpStatusCode.OK
    )
    {
        return new RestResultObject<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Message = message,
            Data = default,
            Errors = null
        };
    }

    /// <summary>
    /// Creates an error result
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code (default is BadRequest)</param>
    /// <param name="errors">Optional list of errors</param>
    /// <returns>A RestResultObject with Success = false</returns>
    public static RestResultObject<T> CreateError(
        string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, List<string> errors = null
    )
    {
        var result = new RestResultObject<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors ?? new List<string>()
        };

        if (errors != null)
        {
            result.Errors.AddRange(errors);
        }

        return result;
    }

    /// <summary>
    /// Creates an error result for a specific exception
    /// </summary>
    /// <param name="ex">The exception that occurred</param>
    /// <param name="message">Optional custom message (default uses exception message)</param>
    /// <param name="statusCode">HTTP status code (default is InternalServerError)</param>
    /// <returns>A RestResultObject with Success = false</returns>
    public static RestResultObject<T> CreateError(
        Exception ex, string message = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError
    )
    {
        return new RestResultObject<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Message = message ?? "An error occurred: " + ex.Message,
            Errors = new List<string> { ex.Message }
        };
    }
}
