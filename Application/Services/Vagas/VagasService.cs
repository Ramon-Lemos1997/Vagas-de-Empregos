using Domain.Entites.User;
using Domain.Entites.Vagas;
using Domain.Interfaces.Infra;
using Domain.Interfaces.Vagas;
using Domain.Models.Infra;
using Domain.Models.Support;
using Domain.Models.Vagas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace Application.Services.Vagas
{
    public class VagasService : IVagas
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPagination _pagination;
        private readonly IEmail _email;
        public VagasService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IPagination pagination, IEmail email)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _pagination = pagination;
            _email = email;
        }

        //------------------------------------------------------------------------------

        /// <summary>
        /// Cria uma nova vaga de emprego.
        /// </summary>
        /// <param name="user">O principal de reivindicações do usuário autenticado.</param>
        /// <param name="model">O modelo de dados para criar uma nova vaga.</param>
        /// <param name="cancellationToken">Um token para cancelar operações assíncronas.</param>
        /// <returns>
        /// Um objeto <see cref="ResultModel"/> indicando o resultado da operação.
        /// </returns>
        /// <remarks>
        /// Este método cria uma nova vaga de emprego com base nos dados fornecidos no modelo.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Lançada quando o modelo fornecido é nulo.
        /// </exception>
        /// <exception cref="Exception">
        /// Lançada em caso de falha durante o processo de criação da vaga.
        /// </exception>
        public async Task<ResultModel> Create(ClaimsPrincipal user, VagaModel request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    return new ResultModel(false, "Nenhum dado recebido.");
                }

                var User = _userManager.GetUserId(user);
                var vaga = new Vaga
                {
                    Title = request.Title,
                    Company = request.Company,
                    Description = request.Description,
                    Remuneration = request.Remuneration,
                    Regime = request.Regime,
                    Benefits = request.Benefits,
                    UserId = User,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.VagaRepository.Add(vaga, cancellationToken);

                int createTask = await _unitOfWork.SaveChanges();
                if (createTask == 0)
                {
                    return HandleError("Interno, Erro ao processar a requisição.");
                }

                return HandleSuccess("Operação bem sucedida.");
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exceção: " + ex.Message);
                //Console.WriteLine("StackTrace: " + ex.StackTrace);
                return HandleError($"Erro durante a operação de salvamento: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém os detalhes de uma vaga de emprego com base no ID fornecido.
        /// </summary>
        /// <param name="id">O identificador único da vaga de emprego.</param>
        /// <param name="cancellationToken">Um token para cancelar operações assíncronas.</param>
        /// <returns>
        /// Uma tupla contendo um objeto <see cref="ResultModel"/> indicando o resultado da operação e uma instância de <see cref="Vaga"/> com os detalhes da vaga, se encontrada.
        /// </returns>
        /// <remarks>
        /// Este método recupera os detalhes de uma vaga de emprego com base no ID fornecido.
        /// </remarks>
        /// <exception cref="Exception">
        /// Lançada em caso de falha durante o processo de recuperação da vaga.
        /// </exception>
        public async Task<(ResultModel, Vaga)> GetById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var vaga = await _unitOfWork.VagaRepository.GetById(id, cancellationToken);
                if (vaga == null || vaga.Id == 0)
                {
                    return (HandleError("Não foi possível encontrar a vaga solicitada."), new Vaga());
                }
                return (HandleSuccess("Vaga encontrada com sucesso"), vaga);
            }
            catch (Exception ex)
            {
                return (HandleError($"Erro durante a operação: {ex.Message}"), new Vaga());
            }
        }

        /// <summary>
        /// Lista todas as vagas de emprego disponíveis em ordem da atual para mais antiga, com suporte para paginação.
        /// </summary>
        /// <param name="page">O número da página a ser exibida.</param>
        /// <param name="cancellationToken">Um token para cancelar operações assíncronas.</param>
        /// <returns>
        /// Uma tupla contendo um objeto <see cref="ResultModel"/> indicando o resultado da operação e uma coleção de <see cref="Vaga"/> representando as vagas disponíveis.
        /// </returns>
        /// <remarks>
        /// Este método lista todas as vagas de emprego disponíveis, oferecendo suporte para paginação.
        /// </remarks>
        /// <exception cref="Exception">
        /// Lançada em caso de falha durante o processo de listagem de vagas.
        /// </exception>
        public async Task<(ResultModel, IEnumerable<Vaga>)> ListAll(int? page, CancellationToken cancellationToken)
        {
            try
            {
                var list = await _unitOfWork.VagaRepository.ListAll(cancellationToken);
                if (list == null || !list.Any())
                {
                    return (HandleError("A lista de vagas está vazia."), Array.Empty<Vaga>());
                }

                list = list.OrderByDescending(v => v.CreatedAt).ToList();
                var pageList = await _pagination.PaginationDataAsync(list, page);
                return (HandleSuccess("Operação bem sucedida."), pageList);
            }
            catch (Exception ex)
            {
                return (HandleError($"Erro durante a operação de listagem: {ex.Message}"), Array.Empty<Vaga>());
            }
        }

        /// <summary>
        /// Envia o currículo do candidato por e-mail para a vaga de emprego correspondente.
        /// </summary>
        /// <param name="curriculum">O arquivo do currículo a ser enviado.</param>
        /// <param name="userEmail">O endereço de e-mail do candidato.</param>
        /// <param name="title">O título da vaga de emprego para a qual o currículo está sendo enviado.</param>
        /// <param name="cancellationToken">Um token para cancelar operações assíncronas.</param>
        /// <returns>
        /// Um objeto <see cref="ResultModel"/> indicando o resultado da operação.
        /// </returns>
        /// <remarks>
        /// Este método verifica se o formato do currículo é suportado (PDF ou DOC) antes de enviá-lo por e-mail.
        /// </remarks>
        /// <exception cref="Exception">
        /// Lançada em caso de falha durante o processo de envio do currículo.
        /// </exception>
        public async Task<ResultModel> SendCurriculum(IFormFile curriculum, string userEmail, string title, CancellationToken cancellationToken)
        {
            try
            {
                if (curriculum != null && curriculum.Length > 0)
                {
                    var fileExtension = Path.GetExtension(curriculum.FileName).ToLower();
                    if (fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".docx")
                    {
                        var model = new SendCurriculumModel
                        {
                            ToEmail = userEmail,
                            Subject = title,
                            Body = $"Você está recebendo o currículo para a vaga {title}",
                            Curriculum = curriculum
                        };

                        var emailSent = await _email.SendCurriculum(model);
                        if (emailSent)
                        {
                            return HandleSuccess("Currículo enviado com sucesso");
                        }

                        return HandleError("Falha ao enviar o currículo por e-mail.");
                    }

                    return HandleError("Formato de currículo não suportado. Envie apenas arquivos PDF ou DOC.");
                }

                return HandleError("Currículo inválido. Verifique se um arquivo foi selecionado.");    
            }
            catch (Exception ex)
            {
                return HandleError($"Erro durante a operação de envio do currículo: {ex.Message}");
            }
        }

        /// <summary>
        /// Recupera todas as vagas associadas a um determinado usuário com base no seu identificador.
        /// </summary>
        /// <param name="user">O objeto ClaimsPrincipal representando o usuário.</param>
        /// <param name="cancellationToken">Um token para cancelar operações assíncronas.</param>
        /// <returns>
        /// Uma tupla contendo um objeto <see cref="ResultModel"/> indicando o resultado da operação
        /// e uma coleção de vagas (<see cref="ICollection{Vaga}"/>) associadas ao usuário.
        /// </returns>
        public async Task<(ResultModel, ICollection<Vaga>)> RetrieveVagasByUserID(ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(user);
            if (userId == null)
            {
                return (HandleError("Usuário não encontrado."), Array.Empty<Vaga>());
            }

            var list = await _unitOfWork.VagaRepository.GetVagasByUserId(userId, cancellationToken);
            if (list == null || !list.Any())
            {
                return (HandleError("Não vagas para listar."), Array.Empty<Vaga>());
            }

            return (HandleSuccess("Operação bem sucedida."), list);
        }

        /// <summary>
        /// Obtém os detalhes de uma vaga de emprego para edição com base no seu identificador.
        /// </summary>
        /// <param name="id">O identificador da vaga de emprego a ser obtida.</param>
        /// <param name="cancellationToken">Um token para cancelar operações assíncronas.</param>
        /// <returns>
        /// Uma tupla contendo um objeto <see cref="ResultModel"/> indicando o resultado da operação
        /// e um modelo de vaga de emprego (<see cref="VagaModel"/>).
        /// </returns>
        public async Task<(ResultModel, VagaModel)> GetVagaForEditAndDeleteById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var vaga = await _unitOfWork.VagaRepository.GetById(id, cancellationToken);
                if (vaga == null || vaga.Id == 0)
                {
                    return (HandleError("Não foi possível encontrar a vaga solicitada."), new VagaModel());
                }

                var vagaModel = ConvertVaga(vaga);
                return (HandleSuccess("Vaga encontrada com sucesso"), vagaModel);
            }
            catch (Exception ex)
            {
                return (HandleError($"Erro durante a operação: {ex.Message}"), new VagaModel());
            }
        }

        /// <summary>
        /// Edita uma vaga de emprego com base nas informações fornecidas no modelo de vaga.
        /// </summary>
        /// <param name="request">O modelo de vaga contendo as informações atualizadas.</param>
        /// <param name="cancellationToken">Um token para cancelar operações assíncronas (opcional).</param>
        /// <returns>
        /// Um objeto <see cref="ResultModel"/> indicando o resultado da operação.
        /// </returns>
        /// <remarks>
        /// Este método verifica se o modelo fornecido é nulo antes de tentar editar a vaga.
        /// </remarks>
        public async Task<ResultModel> Edit(VagaModel request, CancellationToken cancellationToken )
        {
            try
            {
                if (request == null)
                {
                    return HandleError("Nenhum dado recebido.");
                }

                int vagaId = request.Id ?? 0;
               
                var entity = await _unitOfWork.VagaRepository.GetById(vagaId, cancellationToken);             
                if (entity.Id < 0 )
                {
                    return HandleError("Vaga não encontrada.");
                }

                entity.Title = request.Title;
                entity.Company = request.Company;
                entity.Description = request.Description;
                entity.Regime = request.Regime;
                entity.Remuneration = request.Remuneration;
                entity.Benefits = request.Benefits;
                entity.UpdatedAt = DateTime.Now;

                await _unitOfWork.VagaRepository.Update(entity, cancellationToken);

                int saveTask = await _unitOfWork.SaveChanges();
                if (saveTask == 0)
                {
                    return HandleError("Interno, Erro ao processar a requisição.");
                }

                return HandleSuccess("Operação bem sucedida.");
            }
            catch (Exception ex)
            {
                return HandleError($"Erro durante a edição da vaga: {ex.Message}");
            }
        }

        /// <summary>
        /// Exclui uma vaga com base no seu identificador.
        /// </summary>
        /// <param name="vagaId">O identificador único da vaga a ser excluída.</param>
        /// <param name="cancellationToken">Um token para cancelar operações assíncronas.</param>
        /// <returns>
        /// Um objeto <see cref="ResultModel"/> representando o resultado da operação de exclusão.
        /// </returns>
        /// <remarks>
        /// Este método exclui uma vaga com base no identificador fornecido. Retorna um objeto
        /// <see cref="ResultModel"/> indicando se a operação foi bem sucedida ou se houve algum erro.
        /// </remarks>
        public async Task<ResultModel> Delete(int vagaId, CancellationToken cancellationToken)
        {
            try
            {
                if (vagaId == 0 || vagaId == null)
                {
                    return HandleError("Nenhum dado recebido.");
                }
          
                var entity = await _unitOfWork.VagaRepository.GetById(vagaId, cancellationToken);
                if (entity.Id < 0)
                {
                    return HandleError("Vaga não encontrada.");
                }

                await _unitOfWork.VagaRepository.Delete(entity, cancellationToken);

                int saveTask = await _unitOfWork.SaveChanges();
                if (saveTask == 0)
                {
                    return HandleError("Interno, Erro ao processar a requisição.");
                }

                return HandleSuccess("Operação bem sucedida.");
            }
            catch (Exception ex)
            {
                return HandleSuccess($"Erro durante a exclusão do produto: {ex.Message}");
            }
        }

        //------------------------------------------------------------------------------


        /// <summary>
        /// Cria um objeto ResultModel para lidar com erros, usando a mensagem de erro fornecida.
        /// </summary>
        /// <param name="errorMessage">A mensagem de erro a ser incluída no objeto ResultModel.</param>
        /// <returns>O objeto ResultModel com indicador de falha e a mensagem de erro especificada.</returns>
        private ResultModel HandleError(string errorMessage)
            {
                return new ResultModel(false, errorMessage);
            }

        /// <summary>
        /// Cria um objeto ResultModel para lidar com sucesso, usando a mensagem de sucesso fornecida.
        /// </summary>
        /// <param name="errorMessage">A mensagem de sucesso a ser incluída no objeto ResultModel.</param>
        /// <returns>O objeto ResultModel com indicador de falha e a mensagem de sucesso especificada.</returns>
        private ResultModel HandleSuccess(string successMessage)
        {
            return new ResultModel(true, successMessage);
        }

        /// <summary>
        /// Converte uma entidade <see cref="Vaga"/> para um modelo de vaga <see cref="VagaModel"/>.
        /// </summary>
        /// <param name="vaga">A entidade Vaga a ser convertida.</param>
        /// <returns>
        /// Um modelo de vaga <see cref="VagaModel"/> contendo os dados da entidade Vaga.
        /// </returns>
        private VagaModel ConvertVaga(Vaga vaga)
        {
            return new VagaModel 
            {
                Id = vaga.Id,
                Title = vaga.Title,
                Company = vaga.Company,
                Description = vaga.Description,
                Regime = vaga.Regime,
                Remuneration = vaga.Remuneration,
                Benefits = vaga.Benefits,
                UserId = vaga.UserId
            };
        }

        //------------------------------------------------------------------------------

    }
}
