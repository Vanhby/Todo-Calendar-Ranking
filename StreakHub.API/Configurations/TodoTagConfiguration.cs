using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreakHub.API.Models;

namespace StreakHub.API.Configurations
{
    public class TodoTagConfiguration : IEntityTypeConfiguration<TodoTag>
    {
        public void Configure(EntityTypeBuilder<TodoTag> builder)
        {
            builder.ToTable("TodoTags");

            builder.HasKey(x => new { x.TodoId, x.TagId });

            builder.HasOne(x => x.Todo)
                   .WithMany(t => t.TodoTags)
                   .HasForeignKey(x => x.TodoId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Tag)
                   .WithMany(t => t.TodoTags)
                   .HasForeignKey(x => x.TagId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}