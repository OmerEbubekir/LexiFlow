namespace LexiFlow.Application.Common;

public class BaseResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static BaseResponse<T> SuccessResponse(T data)
    {
        return new BaseResponse<T> { Success = true, Data = data };
    }

    public static BaseResponse<T> FailureResponse(string errorMessage)
    {
        return new BaseResponse<T> { Success = false, ErrorMessage = errorMessage };
    }
}

public class BaseResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public static BaseResponse SuccessResponse()
    {
        return new BaseResponse { Success = true };
    }

    public static BaseResponse FailureResponse(string errorMessage)
    {
        return new BaseResponse { Success = false, ErrorMessage = errorMessage };
    }
}
