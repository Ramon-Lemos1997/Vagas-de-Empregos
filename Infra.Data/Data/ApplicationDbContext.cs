using Domain.Entites.User;
using Domain.Entites.Vagas;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Infra.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Vaga> Vagas { get; set; }
        /// <summary>
        /// Método responsável por configurar o modelo do banco de dados.
        /// </summary>
        /// <param name="builder">Builder para configuração do modelo.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Aplica todas as configurações de entidades do assembly atual.
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
