using System.Collections.Generic;

namespace TaskManager.Api.Models
{
    public sealed class ProjectAdmin
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public List<Project> Projects { get; set; } = new List<Project>();

        public ProjectAdmin(User user) 
        {
            UserId = user.Id;
            User = user;
        }
    }
}