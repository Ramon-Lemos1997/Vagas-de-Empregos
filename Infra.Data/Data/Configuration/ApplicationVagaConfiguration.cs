using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entites.Vagas;

namespace Infra.Data.Data.Configuration
{
    public class VagaConfiguration : IEntityTypeConfiguration<Vaga>
    {
        public void Configure(EntityTypeBuilder<Vaga> builder)
        {
            builder.ToTable("Vagas");

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Title).IsRequired().HasMaxLength(255);
            builder.Property(v => v.Company).HasMaxLength(255);
            builder.Property(v => v.Description).IsRequired();
            builder.HasOne(v => v.User)
                .WithMany(u => u.Vagas)
                .HasForeignKey(v => v.UserId)
                .IsRequired();
        }
    }
}
