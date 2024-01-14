using Domain.Entites.User;
using Domain.Models.Support;
using System.Security.Claims;

namespace Domain.Interfaces.Identity
{
    public interface IAccountInterface
    {
        Task<(ResultModel, ApplicationUser user)> CreateUser(string userEmail, string password);
        Task<ResultModel> SendCode(string model);
        Task<ResultModel> CheckIfTokenResetPasswordIsUsed(string userId);
        Task<ResultModel> ResetPassword(string userId, string token, string newPassword);
        Task<ResultModel> UpdatePassword(string newPassowrd, string currPassword, ClaimsPrincipal user);
    }
}
