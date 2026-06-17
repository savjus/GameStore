namespace GameStore.Services;

public class ServiceResult<T>
{
    internal ServiceResult(bool isSuccess, T? value, int statusCode, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        StatusCode = statusCode;
        Error = error;
    }

    public bool IsSuccess { get; }

    public T? Value { get; }

    public int StatusCode { get; }

    public string? Error { get; }
}
