using Microsoft.EntityFrameworkCore;
using StreakHub.API.Models;
using System.Reflection;

namespace StreakHub.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Star> Stars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}