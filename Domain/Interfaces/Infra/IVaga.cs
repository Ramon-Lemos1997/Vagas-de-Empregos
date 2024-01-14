using Domain.Entites.Vagas;

namespace Domain.Interfaces.Infra
{
    public interface IVaga : IRepository<Vaga>
    {
        Task<ICollection<Vaga>> GetVagasByUserId(string userId, CancellationToken cancellationToken);

    }
}
