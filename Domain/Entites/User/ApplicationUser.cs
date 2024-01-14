using System.ComponentModel.DataAnnotations;
using Domain.Entites.Vagas;
using Microsoft.AspNetCore.Identity;


namespace Domain.Entites.User
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [MaxLength(255)]
        public string CompanyName { get; set; }

        [MaxLength(20)]
        public string CNPJ { get; set; }

        public bool ResetPassword { get; set; }

        [MaxLength(100)]
        public string Street { get; set; }

        [MaxLength(100)]
        public string Neighborhood { get; set; }

        [MaxLength(8)]
        public string ZipCode { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string Nation { get; set; }

        [MaxLength(10)]
        public string CompanyNumber { get; set; }

        [MaxLength(30)]
        public string State { get; set; }

        public virtual ICollection<Vaga> Vagas { get; set; }

    }

}

