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
            var user = m_usersService.GetUser(HttpContext.User.Identity.Name);
            if (user.Status == UserStatus.Admin)
            {
                return await m_projectsService.GetAll().ToListAsync();
            }
            else
            {
                return await m_projectsService.GetByUserId(user.Id);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var project = m_projectsService.Get(id);
            return project == null ? NoContent() : Ok(project);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProjectModel projectModel)
        {
            if (projectModel != null)
            {
                var user = m_usersService.GetUser(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                    {
                        var admin = m_db.ProjectAdmins.FirstOrDefault(a => a.UserId == user.Id);
                        if (admin == null)
                        {
                            admin = new ProjectAdmin(user);
                            m_db.ProjectAdmins.Add(admin);
                        }
                        projectModel.AdminId = admin.Id;

                        bool result = m_projectsService.Create(projectModel);
                        return result ? Ok() : NotFound();
                    }
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [HttpPatch("{id}")]
        public IActionResult Update(int id, [FromBody] ProjectModel projectModel)
        {
            if (projectModel != null)
            {
                var user = m_usersService.GetUser(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                    {
                        bool result = m_projectsService.Update(id, projectModel);
                        return result ? Ok() : NotFound();
                    }
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            bool result = m_projectsService.Delete(id);
            return result ? Ok() : NotFound();
        }

        [HttpPatch("{id}/users")]
        public IActionResult AddUsersToProject(int id, [FromBody] List<int> usersIds)
        {
            if (usersIds != null)
            {
                var user = m_usersService.GetUser(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                    {
                        m_projectsService.AddUsersToProject(id, usersIds);
                        return Ok();
                    }
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [HttpPatch("{id}/users/remove/{userId}")]
        public IActionResult RemoveUsersFromProject(int id, [FromBody] List<int> usersIds)
        {
            if (usersIds != null)
            {
                var user = m_usersService.GetUser(HttpContext.User.Identity.Name);
                if (user != null)
                {
                    if (user.Status == UserStatus.Admin || user.Status == UserStatus.Editor)
                    {
                        m_projectsService.RemoveUsersFromProject(id, usersIds);
                        return Ok();
                    }
                }
                return Unauthorized();
            }
            return BadRequest();
        }
    }
}
