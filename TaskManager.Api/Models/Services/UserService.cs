using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TaskManager.Api.Models.Data;

namespace TaskManager.Api.Models.Services
{
    public sealed class UserService
    {
        private readonly ApplicationContext m_db;

        public UserService(ApplicationContext db)
        {
            m_db = db;
        }

        public Tuple<string, string> GetUserLoginPassFromBasicAuth(HttpRequest request)
        {
            string userName = "";
            string userPass = "";
            string authHeader = request.Headers["Autorization"].ToString();
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodedUserNamePass = authHeader.Replace("Basic ", "");
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");

                string[] namePassArray = encoding.GetString(Convert.FromBase64String(encodedUserNamePass)).Split(':');
                userName = namePassArray[0];
                userPass = namePassArray[1];
            }
            return new Tuple<string, string>(userName, userPass);
        }

        public User GetUser(string login, string password)
        {
            User user = m_db.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
            return user;
        }

        public ClaimsIdentity GetIdentity(string username, string password)
        {
            User currentUser = GetUser(username, password);
            if (currentUser != null)
            {
                currentUser.LastLoginDate= DateTime.Now;
                m_db.Users.Update(currentUser);
                m_db.SaveChanges();

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, currentUser.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, currentUser.Status.ToString())
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }
            return null;
        }
    }
}