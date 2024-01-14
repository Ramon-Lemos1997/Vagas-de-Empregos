
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.User
{
    public class EmailModel
    {
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Insira um email válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
