using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreakHub.API.Models;

namespace StreakHub.API.Configurations
{
    public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
    {
        public void Configure(EntityTypeBuilder<Reminder> builder)
        {
            builder.ToTable("Reminders");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsSent).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.HasOne(x => x.Todo)
                   .WithMany(t => t.Reminders)
                   .HasForeignKey(x => x.TaskId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}