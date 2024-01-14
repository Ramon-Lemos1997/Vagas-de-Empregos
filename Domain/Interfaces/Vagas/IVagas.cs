using Domain.Entites.Vagas;
using Domain.Models.Support;
using Domain.Models.Vagas;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Domain.Interfaces.Vagas
{
    public interface IVagas
    {
        Task<ResultModel> Create(ClaimsPrincipal user, VagaModel request, CancellationToken cancellationToken = default);
        Task<(ResultModel, IEnumerable<Vaga>)> ListAll(int? page, CancellationToken cancellationToken = default);
        Task<(ResultModel, Vaga)> GetById(int id, CancellationToken cancellationToken = default);
        Task<ResultModel> SendCurriculum(IFormFile curriculum, string userEmail, string title, CancellationToken cancellationToken = default);
        Task<(ResultModel, ICollection<Vaga>)> RetrieveVagasByUserID(ClaimsPrincipal user, CancellationToken cancellationToken = default);
        Task<(ResultModel, VagaModel)> GetVagaForEditAndDeleteById(int id, CancellationToken cancellationToken = default);
        Task<ResultModel> Edit(VagaModel request, CancellationToken cancellationToken = default);
        Task<ResultModel> Delete(int vagaId, CancellationToken cancellationToken = default);
    }
}
