using Domain.Entites.User;
using Domain.Entities;

namespace Domain.Entites.Vagas
{
    public class Vaga : BaseEntity
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public int Remuneration { get; set; }
        public string Regime { get; set; }
        public string Benefits { get; set; }
        // Propriedades de navegação
        public virtual string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
