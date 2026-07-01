using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreakHub.API.Models;

namespace StreakHub.API.Configurations
{
    public class ShareTagConfiguration : IEntityTypeConfiguration<ShareTag>
    {
        public void Configure(EntityTypeBuilder<ShareTag> builder)
        {
            builder.ToTable("ShareTags");

            builder.HasKey(x => new { x.ShareId, x.TagId });

            builder.HasOne(x => x.Share)
                   .WithMany(s => s.ShareTags)
                   .HasForeignKey(x => x.ShareId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Tag)
                   .WithMany(t => t.ShareTags)
                   .HasForeignKey(x => x.TagId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}