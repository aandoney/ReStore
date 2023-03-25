using Microsoft.EntityFrameworkCore;

namespace API.RequestHelpers;

public class PagedList<T> : List<T>
{
    public PagedList(List<T> items, int count, int PageNumber, int PageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = count,
            PageSize = PageSize,
            CurrentPage = PageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)PageSize)
        };
        AddRange(items);
    }

    public MetaData MetaData { get; set; }

    public static async Task<PagedList<T>> ToPageList(
        IQueryable<T> query, int pageNumber, int pageSize)
    {
        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}