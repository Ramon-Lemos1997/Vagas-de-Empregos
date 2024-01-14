using Microsoft.AspNetCore.Http;

namespace Domain.Models.Infra
{
    public class SendEmailModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; } 
        public IFormFile Curriculum { get; set; }
    }
}
