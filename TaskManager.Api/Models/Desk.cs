using System.Collections.Generic;
using System.Linq;
using TaskManager.Common.Models;
using Newtonsoft.Json;

namespace TaskManager.Api.Models
{
    public sealed class Desk : CommonObject
    {
        public int Id { get; set; }

        public bool IsPrivate { get; set; }

        public string Columns { get; set; }

        public int AdminId { get; set; }

        public User Admin { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public List<Task> Tasks { get; set; } = new List<Task>();

        public Desk() { }

        public Desk(DeskModel deskModel) : base(deskModel)
        {
            Id = deskModel.Id;
            AdminId = deskModel.AdminId;
            IsPrivate = deskModel.IsPrivate;
            ProjectId = deskModel.ProjectId;
            if (deskModel.Columns.Any())
            {
                Columns = JsonConvert.SerializeObject(deskModel.Columns);
            }
        }

        public DeskModel ToDto()
        {
            return new DeskModel()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                CreationDate = CreationDate,
                Photo = Photo,
                AdminId = AdminId,
                IsPrivate = IsPrivate,
                Columns = JsonConvert.DeserializeObject<string[]>(Columns),
                ProjectId = ProjectId
            };
        }
    }
}