namespace MicroServicesOnDocker.Web.WebMvc.ViewModels
{
    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string  Previous { get; set; }
        public string Next { get; set; }
    }
}
