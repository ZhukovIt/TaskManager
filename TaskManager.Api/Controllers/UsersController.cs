using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
