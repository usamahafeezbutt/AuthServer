
namespace AuthServer.Application.Common.Models.Responses
{
    public class ResultResponse<T>(
        bool success,
        string message,
        T result) : BaseResponse(success, message)
    {
        public T? Result { get; set; } = result;
    }
}
