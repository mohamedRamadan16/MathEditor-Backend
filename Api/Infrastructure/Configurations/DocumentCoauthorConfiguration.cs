using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Domain.Entities;

namespace Api.Infrastructure.Configurations
{
    public class DocumentCoauthorConfiguration : IEntityTypeConfiguration<DocumentCoauthor>
    {
        public void Configure(EntityTypeBuilder<DocumentCoauthor> builder)
        {
            builder.Property(dc => dc.UserEmail)
                .IsRequired()
                .HasMaxLength(256);
            builder.Property(dc => dc.CreatedAt)
                .IsRequired();
        }
    }
}
