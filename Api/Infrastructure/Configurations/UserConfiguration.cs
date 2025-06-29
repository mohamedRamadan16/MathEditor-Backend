using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Domain.Entities;

namespace Api.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
            builder.HasIndex(u => u.Email)
                .IsUnique();
            builder.Property(u => u.Handle)
                .HasMaxLength(50);
            builder.HasIndex(u => u.Handle).IsUnique();    
        }
    }
}
