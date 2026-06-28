using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StreakHub.API.Models_Generated;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Reminder> Reminders { get; set; }

    public virtual DbSet<Share> Shares { get; set; }

    public virtual DbSet<Star> Stars { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Todo> Todos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=ep-tiny-thunder-aoe1d14b-pooler.c-2.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_hm6vBdKC1cGX;SslMode=Require;Trust Server Certificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(e => e.ReminderId, "IX_Notifications_ReminderId").IsUnique();

            entity.HasIndex(e => e.TodoId, "IX_Notifications_TodoId");

            entity.HasIndex(e => e.UserId, "IX_Notifications_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Reminder).WithOne(p => p.Notification).HasForeignKey<Notification>(d => d.ReminderId);

            entity.HasOne(d => d.Todo).WithMany(p => p.Notifications).HasForeignKey(d => d.TodoId);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasIndex(e => e.TaskId, "IX_Reminders_TaskId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Task).WithMany(p => p.Reminders).HasForeignKey(d => d.TaskId);
        });

        modelBuilder.Entity<Share>(entity =>
        {
            entity.HasIndex(e => e.ShareCode, "IX_Shares_ShareCode").IsUnique();

            entity.HasIndex(e => e.UserId, "IX_Shares_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ShareCode).HasMaxLength(10);

            entity.HasOne(d => d.User).WithMany(p => p.Shares)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(d => d.Tags).WithMany(p => p.Shares)
                .UsingEntity<Dictionary<string, object>>(
                    "ShareTag",
                    r => r.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    l => l.HasOne<Share>().WithMany().HasForeignKey("ShareId"),
                    j =>
                    {
                        j.HasKey("ShareId", "TagId");
                        j.ToTable("ShareTags");
                        j.HasIndex(new[] { "TagId" }, "IX_ShareTags_TagId");
                    });
        });

        modelBuilder.Entity<Star>(entity =>
        {
            entity.HasIndex(e => e.ShareId, "IX_Stars_ShareId");

            entity.HasIndex(e => e.UserId, "IX_Stars_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Share).WithMany(p => p.Stars).HasForeignKey(d => d.ShareId);

            entity.HasOne(d => d.User).WithMany(p => p.Stars)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Tags_Name").IsUnique();

            entity.Property(e => e.Color).HasDefaultValueSql("'#8b949e'::text");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Todos_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Todos).HasForeignKey(d => d.UserId);

            entity.HasMany(d => d.Tags).WithMany(p => p.Todos)
                .UsingEntity<Dictionary<string, object>>(
                    "TodoTag",
                    r => r.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    l => l.HasOne<Todo>().WithMany().HasForeignKey("TodoId"),
                    j =>
                    {
                        j.HasKey("TodoId", "TagId");
                        j.ToTable("TodoTags");
                        j.HasIndex(new[] { "TagId" }, "IX_TodoTags_TagId");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();

            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("'default.png'::character varying");
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
