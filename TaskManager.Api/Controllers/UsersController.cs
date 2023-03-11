using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class UsersController : ControllerBase
    {
        private readonly ApplicationContext m_db;

        public UsersController(ApplicationContext db) 
        {
            m_db = db;
        }

        [HttpGet("test")]
        public IActionResult TestApi()
        {
            return Ok("Всем привет!");
        }

        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] UserModel userModel)
        {
            if (userModel != null) 
            {
                User newUser = new User(userModel.FirstName, userModel.LastName, userModel.Email,
                    userModel.Password, userModel.Status, userModel.Phone, userModel.Photo);
                m_db.Users.Add(newUser);
                m_db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPatch("update/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserModel userModel) 
        {
            if (userModel != null) 
            {
                User userForUpdate = m_db.Users.FirstOrDefault(user => user.Id == id);
                if (userForUpdate != null) 
                {
                    userForUpdate.FirstName= userModel.FirstName;
                    userForUpdate.LastName = userModel.LastName;
                    userForUpdate.Email= userModel.Email;
                    userForUpdate.Password= userModel.Password;
                    userForUpdate.Phone= userModel.Phone;
                    userForUpdate.Photo= userModel.Photo;
                    userForUpdate.Status = userModel.Status;

                    m_db.Users.Update(userForUpdate);
                    m_db.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest();
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            User user = m_db.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                m_db.Remove(user);
                m_db.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpPost("create/all")]
        public async Task<IActionResult> CreateMultipleUsers([FromBody] List<UserModel> userModels)
        {
            if (userModels != null && userModels.Count > 0)
            {
                var newUsers = userModels.Select(u => new User(u)).ToList();
                await m_db.Users.AddRangeAsync(newUsers);
                await m_db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            return await m_db.Users.Select(u => u.ToDto()).ToListAsync();
        }
    }
}
