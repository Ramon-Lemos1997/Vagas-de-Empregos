using Domain.Entites.Vagas;
using Domain.Models.Support;
using Domain.Models.Vagas;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Domain.Interfaces.Vagas
{
    public interface IVagas
    {
        Task<ResultModel> Create(ClaimsPrincipal user, CreateVagaModel request, CancellationToken cancellationToken = default);
        Task<(ResultModel, IEnumerable<Vaga>)> ListAll(int? page, CancellationToken cancellationToken = default);
        Task<(ResultModel, Vaga)> GetById(int id, CancellationToken cancellationToken = default);
        Task<ResultModel> SendCurriculum(IFormFile curriculum, string userEmail, string title, CancellationToken cancellationToken = default);
    }
}
