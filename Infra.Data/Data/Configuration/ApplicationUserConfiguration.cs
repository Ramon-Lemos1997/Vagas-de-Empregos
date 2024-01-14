using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entites.User;


namespace Infra.Data.Data.Configuration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("AspNetUsers");

            builder.Property(u => u.CompanyName).HasMaxLength(255);
            builder.Property(u => u.CNPJ).HasMaxLength(20);
            builder.Property(u => u.ResetPassword);
            builder.Property(u => u.Street).HasMaxLength(100);
            builder.Property(u => u.Neighborhood).HasMaxLength(100);
            builder.Property(u => u.ZipCode).HasMaxLength(8);
            builder.Property(u => u.City).HasMaxLength(50);
            builder.Property(u => u.State).HasMaxLength(30);
            builder.Property(u => u.CreationDate).HasMaxLength(30);
            builder.Property(u => u.UpdatedDate).HasMaxLength(30);
            builder.HasMany(u => u.Vagas) 
               .WithOne(v => v.User)    
               .HasForeignKey(v => v.UserId);
        }
    }
}
