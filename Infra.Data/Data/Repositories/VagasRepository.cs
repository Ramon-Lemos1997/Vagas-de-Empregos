using Domain.Entites.Vagas;
using Domain.Interfaces.Infra;
using Infra.Data.Context;

namespace Infra.Data.Data.Repositories
{
    public class VagasRepository : Repository<Vaga>, IVaga
    {
        public VagasRepository(ApplicationDbContext dbContext) : base(dbContext)
        { }
    }
}
