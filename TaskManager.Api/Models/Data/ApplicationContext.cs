using Microsoft.EntityFrameworkCore;
using System.Linq;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Data
{
    public sealed class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<ProjectAdmin> ProjectAdmins { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Desk> Desks { get; set; }

        public DbSet<Task> Tasks { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) 
        {
            Database.EnsureCreated();
            if (!Users.Any(user => user.Status == UserStatus.Admin))
            {
                User admin = new User("Victor", "Zhukov", "victor777jt@yandex.ru", "q12werty", UserStatus.Admin);
                Users.Add(admin);
                SaveChanges();
            }
        }
    }
}