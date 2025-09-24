using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Token)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(r => r.JwtId)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(r => r.ReplacedByToken)
            .HasMaxLength(500);
            
        builder.Property(r => r.RevokedReason)
            .HasMaxLength(500);
            
        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(r => r.Token);
        builder.HasIndex(r => new { r.UserId, r.IsUsed, r.IsRevoked });
    }
}