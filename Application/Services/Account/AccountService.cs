using Microsoft.AspNetCore.Identity;
using Domain.Entites.User;
using Domain.Interfaces.Identity;
using Domain.Models.Support;

namespace Application.Services.Account
{
    public class AccountService : IAccountInterface
    {

        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService( UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        //__________________________________________________________________________________________________________

        /// <summary>
        /// Cria um novo usuário com o email especificado e a senha fornecida.
        /// </summary>
        /// <param name="userEmail">O email do usuário.</param>
        /// <param name="password">A senha do usuário.</param>
        /// <returns>Uma tupla contendo um <see cref="OperationResultModel"/> indicando o resultado da operação e um <see cref="ApplicationUser"/>.</returns>
        public async Task<(ResultModel, ApplicationUser user)> CreateUserAsync(string userEmail, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(password))
                {
                    return (new ResultModel(false, "Nenhum dado recebido."), new ApplicationUser());
                }

                // Obtém a data e hora atual no fuso horário do Brasil
                TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                DateTime brazilDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brazilTimeZone);

                var currUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    CreationDate = brazilDateTime,
                    CompanyName = string.Empty,
                    CNPJ = string.Empty,
                    City = string.Empty,
                    Street = string.Empty,
                    Neighborhood = string.Empty,
                    ZipCode = string.Empty,
                    Nation = string.Empty,
                    CompanyNumber = string.Empty,
                    State = string.Empty
                };

                var result = await _userManager.CreateAsync(currUser, password);
                if (result.Succeeded)
                {
                    return (HandleSuccess("Usuário criado com sucesso"), currUser);
                }

                var errors = result.Errors.Select(e => e.Description);
                var errorMessage = string.Join(Environment.NewLine, errors);

                return (HandleError(errorMessage), new ApplicationUser());
            }
            catch (Exception ex)
            {
                return (HandleError($"Exceção não planejada: {ex.Message}"), new ApplicationUser());
            }
        }


        //------------------------------------------------------------------------------------------------------------

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

        //----------------------------------------------------------------------------------------------------------------
    }
}
