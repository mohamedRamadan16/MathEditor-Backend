using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Api.Domain.Entities;

namespace Api.Infrastructure.Configurations
{
    public class RevisionConfiguration : IEntityTypeConfiguration<Revision>
    {
        public void Configure(EntityTypeBuilder<Revision> builder)
        {
            builder.Property(r => r.Data)
                .IsRequired();
            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
