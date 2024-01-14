

namespace Domain.Interfaces.Infra
{
    public interface IUnitOfWork : IDisposable
    {
        IVaga VagaRepository { get; }
        Task<int> SaveChanges();
        Task BeginTransaction();
        Task Commit();
        Task Rollback();
    }
}
