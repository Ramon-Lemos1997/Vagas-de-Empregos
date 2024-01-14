using Domain.Entites.Vagas;
using Domain.Interfaces.Infra;
using Infra.Data.Context;

namespace Infra.Data.Data.Repositories
{
    public class VagasRepository : Repository<Vaga>, IVaga
    {
        public VagasRepository(ApplicationDbContext dbContext) : base(dbContext)
        { }

        /// <summary>
        /// Obtém todas as vagas associadas a um usuário com base no ID do usuário.
        /// </summary>
        /// <param name="userId">O ID do usuário.</param>
        /// <param name="cancellationToken">Um token para cancelar operações assíncronas.</param>
        /// <returns>Uma coleção de vagas associadas ao usuário.</returns>
        public async Task<ICollection<Vaga>> GetVagasByUserId(string userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                await _context.Entry(user).Collection(u => u.Vagas).LoadAsync(cancellationToken);
                return user.Vagas ?? Enumerable.Empty<Vaga>().ToList();
            }

            return Enumerable.Empty<Vaga>().ToList();
        }
    }
}
