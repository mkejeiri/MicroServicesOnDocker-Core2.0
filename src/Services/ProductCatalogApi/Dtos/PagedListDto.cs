using System.Collections.Generic;

namespace ProductCatalogApi.Dtos
{
    public class PagedListDto<T> where T: class
    {
        public PagedListDto(int totalCount, int currentPage, int totalPages, int pageSize, IList<T> items)
        {
            TotalCount = totalCount;
            CurrentPage = currentPage;
            TotalPages = totalPages;
            PageSize = pageSize;
            Items = items;
        }

        private PagedListDto() { }
        public int TotalCount { get; }
        public int CurrentPage { get; }
        public int TotalPages { get; }
        public int PageSize { get;  }
        public IList<T> Items {get;}
    }
}
