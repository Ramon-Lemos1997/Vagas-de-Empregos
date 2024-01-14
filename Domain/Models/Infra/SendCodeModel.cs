
namespace Domain.Models.Infra
{
    public class SendCodeModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}