using FitnessTracker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.DataAccess.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.Id).IsRequired().HasMaxLength(36);
        builder.Property(x => x.Token).IsRequired();
        builder.HasIndex(x => x.Token).IsUnique();
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(36);
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.HasOne(r=>r.User).WithMany().HasForeignKey(r=>r.UserId);
    }
}