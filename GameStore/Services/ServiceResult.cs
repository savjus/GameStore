namespace GameStore.Services;

public static class ServiceResult
{
    public static ServiceResult<T> Success<T>(T value, int statusCode)
    {
        return new ServiceResult<T>(true, value, statusCode, null);
    }

    public static ServiceResult<T> Fail<T>(int statusCode, string error)
    {
        return new ServiceResult<T>(false, default, statusCode, error);
    }
}
