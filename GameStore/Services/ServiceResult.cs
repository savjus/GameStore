namespace GameStore.Services;

public class ServiceResult
{
    internal ServiceResult(bool isSuccess, int statusCode, string? error)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Error = error;
    }

    public bool IsSuccess { get; }

    public int StatusCode { get; }

    public string? Error { get; }

    public static ServiceResult Success(int statusCode)
    {
        return new ServiceResult(true, statusCode, null);
    }

    public static ServiceResult<T> Success<T>(T value, int statusCode)
    {
        return new ServiceResult<T>(true, value, statusCode, null);
    }

    public static ServiceResult Fail(int statusCode, string error)
    {
        return new ServiceResult(false, statusCode, error);
    }

    public static ServiceResult<T> Fail<T>(int statusCode, string error)
    {
        return new ServiceResult<T>(false, default, statusCode, error);
    }
}
