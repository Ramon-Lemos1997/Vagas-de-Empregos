using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using Domain.Models.Infra;
using Domain.Interfaces.Infra;

namespace Infra.Data.Support.Email
{
    public class EmailService : IEmail
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //___________________________________________________________________


        /// <summary>
        /// Envia um e-mail com anexo usando a API SendGrid.
        /// </summary>
        /// <param name="model">O modelo de e-mail a ser enviado, incluindo o currículo como anexo.</param>
        /// <returns>
        /// Verdadeiro se o e-mail for enviado com sucesso, caso contrário, falso.
        /// </returns>
        /// <remarks>
        /// O método utiliza a API SendGrid para enviar um e-mail com anexo (PDF, DOC ou DOCX).
        /// </remarks>
        /// <exception cref="Exception">Exceção lançada em caso de falha durante o processo de envio do e-mail.</exception>
        public async Task<bool> SendCurriculum(SendCurriculumModel model)
        {
            try
            {
                var apiKey = _configuration["SendGridSettings:Key"];
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("lemosramonteste1997@gmail.com", "Ramon Lemos");
                var to = new EmailAddress(model.ToEmail);
                var msg = new SendGridMessage
                {
                    From = from,
                    Subject = model.Subject,
                };

                msg.HtmlContent = model.Body;
          
                var memoryStream = new MemoryStream();
                await model.Curriculum.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var fileName = Path.GetFileName(model.Curriculum.FileName);

                var fileExtension = Path.GetExtension(fileName).ToLower();
                if (fileExtension == ".pdf")
                {
                    msg.AddAttachment(fileName, Convert.ToBase64String(memoryStream.ToArray()), "application/pdf", "attachment");
                }
                else if (fileExtension == ".doc")
                {
                    msg.AddAttachment(fileName, Convert.ToBase64String(memoryStream.ToArray()), "application/msword", "attachment");
                }
                else if (fileExtension == ".docx")
                {
                    msg.AddAttachment(fileName, Convert.ToBase64String(memoryStream.ToArray()), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "attachment");
                }

                msg.AddTo(to);

                var response = await client.SendEmailAsync(msg);
                return response.StatusCode == System.Net.HttpStatusCode.Accepted;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Envia um e-mail usando a API SendGrid.
        /// </summary>
        /// <param name="model">O modelo de e-mail a ser enviado.</param>
        /// <returns>Verdadeiro se o e-mail for enviado com sucesso, caso contrário, falso.</returns>
        public async Task<bool> SendCode(SendCodeModel model)
        {
            try
            {
                var apiKey = _configuration["SendGridSettings:Key"];
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("lemosramonteste1997@gmail.com", "Ramon Lemos");
                var to = new EmailAddress(model.ToEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, model.Subject, plainTextContent: null, htmlContent: model.Message);

                var response = await client.SendEmailAsync(msg);
                return response.StatusCode == System.Net.HttpStatusCode.Accepted;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //-----------------------------------------------------------------------------------------------------------
    }
}