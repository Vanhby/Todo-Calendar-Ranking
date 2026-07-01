using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreakHub.API.Models;

namespace StreakHub.API.Configurations
{
    public class StarConfiguration : IEntityTypeConfiguration<Star>
    {
        public void Configure(EntityTypeBuilder<Star> builder)
        {
            builder.ToTable("Stars");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(x => x.User)
                   .WithMany(u => u.Stars)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Share)
                   .WithMany(s => s.Stars)
                   .HasForeignKey(x => x.ShareId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}