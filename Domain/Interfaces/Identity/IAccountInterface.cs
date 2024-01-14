using Domain.Entites.User;
using Domain.Models.Support;

namespace Domain.Interfaces.Identity
{
    public interface IAccountInterface
    {
        Task<(ResultModel, ApplicationUser user)> CreateUserAsync(string userEmail, string password);    

    }
}
