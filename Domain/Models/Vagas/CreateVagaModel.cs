
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Vagas
{
    public class CreateVagaModel
    {
        [Required(ErrorMessage = "O campo Título é obrigatório.")]
        [Display(Name = "Título")]
        public string Title { get; set; }

        [Required(ErrorMessage = "O campo Empresa é obrigatório.")]
        [Display(Name = "Empresa")]
        public string Company { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O campo Regime é obrigatório.")]
        [Display(Name = "Regime")]
        public string Regime { get; set; }

        [Required(ErrorMessage = "O campo Remuneração é obrigatório.")]
        [Range(1, 1000000, ErrorMessage = "A Remuneração deve estar entre 1 e 1,000,000.")]
        [Display(Name = "Remuneração")]
        public int Remuneration { get; set; }

        [Required(ErrorMessage = "O campo Benefícios é obrigatório.")]
        [Display(Name = "Benefícios")]
        public string Benefits { get; set; }
    }
}