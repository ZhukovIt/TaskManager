using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TaskManager.Api.Models.Data;
using System.Linq;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AccountController : ControllerBase
    {
        private readonly ApplicationContext m_db;
        private readonly UserService m_userService;

        public AccountController(ApplicationContext db)
        {
            m_db = db;
            m_userService = new UserService(m_db);
        }

        [HttpGet("info")]
        public IActionResult GetCurrentUserInfo()
        {
            string userName = HttpContext.User.Identity.Name;
            User user = m_db.Users.FirstOrDefault(u => u.Email == userName);
            if (user != null)
            {
                return Ok(user.ToDto());
            }
            return NotFound();
        }

        [HttpPost("token")]
        public IActionResult GetToken()
        {
            var userData = m_userService.GetUserLoginPassFromBasicAuth(Request);
            string login = userData.Item1;
            string pass = userData.Item2;
            var identity = m_userService.GetIdentity(login, pass);

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Ok(response);
        }
    }
}