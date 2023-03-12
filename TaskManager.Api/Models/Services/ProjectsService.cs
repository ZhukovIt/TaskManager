using System.Linq;
using TaskManager.Api.Models.Abstractions;
using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services
{
    public sealed class ProjectsService : AbstractionService, ICommonService<ProjectModel>
    {
        public ProjectsService(ApplicationContext db) : base(db)
        {

        }

        public bool Create(ProjectModel model)
        {
            bool result = DoAction(() =>
            {
                Project project = new Project(model);
                m_db.Projects.Add(project);
                m_db.SaveChanges();
            });
            return result;
        }

        public bool Delete(int id)
        {          
            Project project = m_db.Projects.FirstOrDefault(p => p.Id == id);
            if (project != null)
            {
                bool result = DoAction(() =>
                {
                    m_db.Remove(project);
                    m_db.SaveChanges();
                });
                return result;
            }
            return false;
        }

        public bool Update(int id, ProjectModel model)
        {
            Project project = m_db.Projects.FirstOrDefault(p => p.Id == id);
            if (project != null)
            {
                bool result = DoAction(() =>
                {
                    project.Name = model.Name;
                    project.Description = model.Description;
                    project.Photo = model.Photo;
                    project.Status = model.Status;
                    project.AdminId = model.AdminId;
                    m_db.Projects.Update(project);
                    m_db.SaveChanges();
                });
                return result;
            }
            return false;
        }
    }
}
