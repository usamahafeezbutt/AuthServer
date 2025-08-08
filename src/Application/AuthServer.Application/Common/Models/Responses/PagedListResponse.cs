namespace AuthServer.Application.Common.Models.Responses
{
    public class PagedListResponse<T>(
        bool success,
        string message,
        T result,
        int totalRecords) : ResultResponse<T>(success, message, result)
    {
        public int TotalRecords { get; set; } = totalRecords;
    }
}
