using Microsoft.AspNetCore.Identity;
using Domain.Entites.User;
using Domain.Interfaces.Identity;
using Domain.Models.Support;
using Domain.Models.Infra;
using Domain.Interfaces.Infra;
using System.Security.Claims;

namespace Application.Services.Account
{
    public class AccountService : IAccountInterface
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmail _email;

        public AccountService(UserManager<ApplicationUser> userManager, IEmail email)
        {
            _userManager = userManager;
            _email = email;
        }


        //__________________________________________________________________________________________________________

        /// <summary>
        /// Cria um novo usuário com o email especificado e a senha fornecida.
        /// </summary>
        /// <param name="userEmail">O email do usuário.</param>
        /// <param name="password">A senha do usuário.</param>
        /// <returns>Uma tupla contendo um <see cref="ResultModel"/> indicando o resultado da operação e um <see cref="ApplicationUser"/>.</returns>
        public async Task<(ResultModel, ApplicationUser user)> CreateUser(string userEmail, string password)
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

        /// <summary>
        /// Envia um código de redefinição de senha para o email especificado.
        /// </summary>
        /// <param name="userEmail">O endereço de email do usuário para o qual o código será enviado.</param>
        /// <returns>Um objeto <see cref="ResultModel"/> que indica se o email foi enviado com sucesso.</returns>
        public async Task<ResultModel> SendCode(string userEmail)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    return HandleError("Nenhum dado recebido.");
                }

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return HandleError("Usuário não encontrado.");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var scheme = "https";
                var host = "localhost:7212";
                var link = $"{scheme}://{host}/Identity/Account/ResetPassword?token={Uri.EscapeDataString(token)}&userId={Uri.EscapeDataString(user.Id)}";

                var subject = "Redefinição de Senha";
                var message = $"Clique <a href=\"{link}\">aqui</a> para redefinir sua senha.";
                var model = new SendCodeModel
                {
                    ToEmail = user.Email,
                    Subject = subject,
                    Message = message
                };

                var emailSent = await _email.SendCode(model);
                if (emailSent)
                {
                    if (user.ResetPassword)
                    {
                        user.ResetPassword = false;
                        await _userManager.UpdateAsync(user);
                    }
                    return HandleSuccess("Email enviado com sucesso.");
                }

                return HandleError("Houve algum erro entre a camada de service e de infraestrutura.");
            }
            catch (Exception ex)
            {
                return HandleError($"Exceção não planejada: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se o token de redefinição de senha do usuário foi usado.
        /// </summary>
        /// <param name="userId">O identificador exclusivo do usuário.</param>
        /// <returns>Um objeto <see cref="ResultModel"/> indicando se o token foi usado.</returns>
        public async Task<ResultModel> CheckIfTokenResetPasswordIsUsed(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return HandleError("Usuário não encontrado.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user.ResetPassword)
                {
                    return HandleError("Este link já foi usado.");
                }

                return HandleSuccess("Sucesso.");
            }
            catch (Exception ex)
            {
                return HandleError($"Exceção não planejada: {ex.Message}");
            }
        }

        /// <summary>
        /// Redefine a senha do usuário com o token fornecido e a nova senha especificada.
        /// </summary>
        /// <param name="userId">O ID do usuário cuja senha será redefinida.</param>
        /// <param name="token">O token usado para redefinir a senha.</param>
        /// <param name="newPassword">A nova senha a ser definida para o usuário.</param>
        /// <returns>Um objeto <see cref="ResultModel"/> que indica se a redefinição de senha foi bem-sucedida.</returns>
        public async Task<ResultModel> ResetPassword(string userId, string token, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return HandleError("Nenhum dado recebido.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return HandleError("Usuário não encontrado.");
                }

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    user.ResetPassword = true;
                    await _userManager.UpdateAsync(user);
                    return HandleSuccess("Senha redefinida com sucesso.");
                }

                return HandleError("Falha em redefinir senha.");
            }
            catch (Exception ex)
            {
                return HandleError($"Exceção não planejada: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza a senha do usuário com a nova senha fornecida, desde que a senha atual também seja fornecida corretamente.
        /// </summary>
        /// <param name="newPassword">A nova senha desejada para o usuário.</param>
        /// <param name="currPassword">A senha atual do usuário.</param>
        /// <param name="user">O principal de reivindicações do usuário.</param>
        /// <returns>Um objeto <see cref="ResultModel"/> que indica se a senha foi atualizada com sucesso ou se houve uma falha.</returns>
        public async Task<ResultModel> UpdatePassword(string newPassword, string currPassword, ClaimsPrincipal user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(currPassword) || user == null)
                {
                    return HandleError("Nenhum dado recebido.");
                }

                var usedId = _userManager.GetUserId(user);
                if (usedId == null)
                {
                    return HandleError("Usuário não encontrado.");
                }

                var currUser = await _userManager.FindByIdAsync(usedId);
                if (currUser == null)
                {
                    return HandleError("Usuário não encontrado.");
                }
                var passwordCorrect = await _userManager.CheckPasswordAsync(currUser, currPassword);
                if (!passwordCorrect)
                {
                    return HandleError("A senha atual está incorreta. Verifique e tente novamente.");
                }

                var result = await _userManager.ChangePasswordAsync(currUser, currPassword, newPassword);
                if (result.Succeeded)
                {
                    return HandleSuccess("Senha atualizada com sucesso.");
                }

                return HandleError("Falha ao atualizar a senha.");
            }
            catch (Exception ex)
            {
                return HandleError($"Exceção não planejada: {ex.Message}");
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
