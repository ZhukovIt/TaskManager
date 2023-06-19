using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public ProjectModel Get(int id)
        {
            Project project = m_db.Projects.Include(p => p.AllUsers).Include(p => p.AllDesks).FirstOrDefault(p => p.Id == id);
            var projectModel = project?.ToDto();
            if (projectModel != null)
            {
                projectModel.AllUsersIds = project.AllUsers.Select(u => u.Id).ToList();
                projectModel.AllDesksIds = project.AllUsers.Select(u => u.Id).ToList();
            }
            return projectModel;
        }

        public async Task<IEnumerable<ProjectModel>> GetByUserId(int userId)
        {
            List<ProjectModel> result = new List<ProjectModel>();

            var admin = m_db.ProjectAdmins.FirstOrDefault(a => a.UserId == userId);
            if (admin != null)
            {
                var projectsForAdmin = await m_db.Projects.Where(p => p.AdminId == admin.Id).Select(p => p.ToDto()).ToListAsync();
                result.AddRange(projectsForAdmin);
            }
            var projectsForUser = await m_db.Projects.Include(p => p.AllUsers).Where(p => p.AllUsers.Any(u => u.Id == userId)).Select(p => p.ToDto()).ToListAsync();
            result.AddRange(projectsForUser);
            return result;
        }

        public IQueryable<CommonModel> GetAll()
        {
            return m_db.Projects.Select(p => p.ToDto() as CommonModel);
        }

        public void AddUsersToProject(int id, List<int> userIds)
        {
            Project project = m_db.Projects.FirstOrDefault(p => p.Id == id);
            foreach (int userId in userIds)
            {
                var user = m_db.Users.FirstOrDefault(u => u.Id == userId);
                if (!project.AllUsers.Contains(user))
                {
                    project.AllUsers.Add(user);
                }
            }
            m_db.SaveChanges();
        }

        public void RemoveUsersFromProject(int id, List<int> userIds)
        {
            Project project = m_db.Projects.Include(p => p.AllUsers).FirstOrDefault(p => p.Id == id);
            foreach (int userId in userIds)
            {
                var user = m_db.Users.FirstOrDefault(u => u.Id == userId);
                if (project.AllUsers.Contains(user))
                {
                    project.AllUsers.Remove(user);
                }
            }
            m_db.SaveChanges();
        }
    }
}
