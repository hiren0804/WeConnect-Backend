namespace WeConnect.Application.Common.Models;

/// <summary>
/// WeConnect standard envelope — every endpoint returns this shape.
/// Success:  { success:true,  data:T,    message, meta }
/// Failure:  { success:false, data:null, message, errors }
/// </summary>
public class ApiResponse<T>
{
    public bool      Success   { get; init; }
    public string    Message   { get; init; }  = string.Empty;
    public T?        Data      { get; init; }
    public object?   Meta      { get; init; }
    public IEnumerable<ApiError>? Errors { get; init; }
    public string    TraceId   { get; init; }  = string.Empty;
    public DateTime  Timestamp { get; init; }  = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string message = "Success", object? meta = null)
        => new() { Success = true,  Data = data, Message = message, Meta = meta };

    public static ApiResponse<T> Fail(string message, IEnumerable<ApiError>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

// Non-generic version for empty/void responses — does NOT inherit ApiResponse<T>
public class ApiResponse
{
    public bool      Success   { get; init; }
    public string    Message   { get; init; }  = string.Empty;
    public object?   Meta      { get; init; }
    public IEnumerable<ApiError>? Errors { get; init; }
    public string    TraceId   { get; init; }  = string.Empty;
    public DateTime  Timestamp { get; init; }  = DateTime.UtcNow;

    public static ApiResponse NoContent(string message = "Done")
        => new() { Success = true, Message = message };

    public static ApiResponse Fail(string message, IEnumerable<ApiError>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

public sealed record ApiError(string Field, string Message, string? Code = null);

public sealed record PaginationMeta(
    int Page, int PageSize, int TotalCount, int TotalPages,
    bool HasNext, bool HasPrevious);