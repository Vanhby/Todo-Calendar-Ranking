using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreakHub.API.Models;

namespace StreakHub.API.Configurations
{
    public class ShareConfiguration : IEntityTypeConfiguration<Share>
    {
        public void Configure(EntityTypeBuilder<Share> builder)
        {
            builder.ToTable("Shares");
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ShareCode).IsUnique();
            builder.Property(x => x.ShareCode).IsRequired().HasMaxLength(10);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(x => x.User)
                   .WithMany(u => u.Shares)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}