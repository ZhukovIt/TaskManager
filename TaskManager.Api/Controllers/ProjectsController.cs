using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Data;
using TaskManager.Api.Models.Services;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public sealed class ProjectsController : ControllerBase
    {
        private readonly ApplicationContext m_db;
        private readonly UsersService m_usersService;
        private readonly ProjectsService m_projectsService;

        public ProjectsController(ApplicationContext db)
        {
            m_db = db;
            m_usersService= new UsersService(db);
            m_projectsService= new ProjectsService(db);
        }

        [HttpGet]
        public async Task<IEnumerable<ProjectModel>> GetProjects()
        {
            return await m_db.Projects.Select(p => p.ToDto()).ToListAsync();
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProjectModel projectModel)
        {
            if (projectModel != null)
            {
                var user = m_usersService.GetUser(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    var admin = m_db.ProjectAdmins.FirstOrDefault(a => a.UserId == user.Id);
                    if (admin == null)
                    {
                        admin = new ProjectAdmin(user);
                        m_db.ProjectAdmins.Add(admin);
                    }
                    projectModel.AdminId = admin.Id;
                }
                bool result = m_projectsService.Create(projectModel);
                return result ? Ok() : NotFound();
            }
            return BadRequest();
        }

        [HttpPatch("{id}")]
        public IActionResult Update(int id, [FromBody] ProjectModel projectModel)
        {
            if (projectModel != null)
            {
                bool result = m_projectsService.Update(id, projectModel);
                return result ? Ok() : NotFound();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool result = m_projectsService.Delete(id);
            return result ? Ok() : NotFound();
        }
    }
}
