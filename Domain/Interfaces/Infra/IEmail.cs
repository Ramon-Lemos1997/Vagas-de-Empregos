using Domain.Models.Infra;

namespace Domain.Interfaces.Infra
{
    public interface IEmail
    {
        Task<bool> SendCurriculum(SendCurriculumModel model);
        Task<bool> SendCode(SendCodeModel model);
    }
}
