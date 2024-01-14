using Domain.Entites.Vagas;
using Microsoft.AspNetCore.Identity;


namespace Domain.Entites.User
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool ResetPassword { get; set; }
        public virtual ICollection<Vaga> Vagas { get; set; }

    }

}

