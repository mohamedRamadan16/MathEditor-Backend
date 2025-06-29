using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Domain.Entities;

namespace Api.Infrastructure.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(d => d.Handle)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(d => d.Handle)
                .IsUnique();
            builder.Property(d => d.CreatedAt)
                .IsRequired();
            builder.Property(d => d.UpdatedAt)
                .IsRequired();
            builder.Property(d => d.Id)
                .HasDefaultValueSql("NEWID()");
        }
    }
}
