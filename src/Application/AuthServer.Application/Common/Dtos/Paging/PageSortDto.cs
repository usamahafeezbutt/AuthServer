namespace AuthServer.Application.Common.Dtos.Paging
{
    public class PageSortDto : PageDto
    {
        public string? SortBy { get; set; }
        public bool SortOrder { get; set; }
    }


}
