using X.PagedList;

namespace Domain.Interfaces.Infra
{
    public interface IPagination
    {
        Task<IPagedList<T>> PaginationDataAsync<T>(IEnumerable<T> data, int? page);
    }
}
