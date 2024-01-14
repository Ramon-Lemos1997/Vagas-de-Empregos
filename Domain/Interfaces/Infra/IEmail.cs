using Domain.Models.Infra;

namespace Domain.Interfaces.Infra
{
    public interface IEmail
    {
        Task<bool> SendCurriculumAsync(SendEmailModel model);
    }
}
