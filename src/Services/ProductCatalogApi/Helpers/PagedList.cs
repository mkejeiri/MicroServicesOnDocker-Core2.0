using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalogApi.Helpers
{
    public class PagedList<TEntity> : List<TEntity>  where TEntity : class
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        private PagedList(){}
        private PagedList(List<TEntity> items, int totalCount, int pageNumber, int pageSize)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(totalCount / (double)(pageSize));
            this.AddRange(items);
        }

        /* N O T E :
            IQueryable<T>: it differ execution of our request to get a set of T (e.g users)  
            and it allows us to define  part of our query against the DB in multiple steps and 
            with differed execution, e.g in this particular case we will skip a number of items and take
            another numbers of items that matches the page number and page size, which ultimately allow us to page 
            our request!!!*/
        public static async Task<PagedList<TEntity>> CreateAsync(IQueryable<TEntity> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<TEntity>(items, count, pageNumber, pageSize);
        }
    }
}
