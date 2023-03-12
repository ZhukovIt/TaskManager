using System.Collections.Generic;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models
{
    public sealed class Project : CommonObject
    {
        public int Id { get; set; }

        public int? AdminId { get; set; }

        public ProjectAdmin Admin { get; set; }

        public List<User> AllUsers { get; set; } = new List<User>();

        public List<Desk> AllDesks { get; set; } = new List<Desk>();

        public ProjectStatus Status { get; set; }

        public Project() { }

        public Project(ProjectModel projectModel) : base(projectModel)
        {
            Id = projectModel.Id;
            AdminId = projectModel.AdminId;
            Status= projectModel.Status;
        }

        public ProjectModel ToDto()
        {
            return new ProjectModel()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                CreationDate = CreationDate,
                Photo = Photo,
                AdminId = AdminId,
                Status = Status
            };
        }
    }
}
