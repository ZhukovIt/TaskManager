using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Api.Models.Abstractions;
using TaskManager.Api.Models.Data;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Services
{
    public sealed class UsersService : AbstractionService, ICommonService<UserModel>
    {
        public UsersService(ApplicationContext db) : base(db)
        {
            
        }

        public Tuple<string, string> GetUserLoginPassFromBasicAuth(HttpRequest request)
        {
            string userName = "";
            string userPass = "";
            string authHeader = request.Headers["Authorization"].ToString();
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

        public User GetUser(string login)
        {
            User user = m_db.Users.FirstOrDefault(u => u.Email == login);
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

        public bool Create(UserModel model)
        {
            return DoAction(() =>
            {
                User newUser = new User(model.FirstName, model.LastName, model.Email,
                    model.Password, model.Status, model.Phone, model.Photo);
                m_db.Users.Add(newUser);
                m_db.SaveChanges();
            });
        }

        public bool Update(int id, UserModel model)
        {
            User userForUpdate = m_db.Users.FirstOrDefault(user => user.Id == id);
            if (userForUpdate != null)
            {
                return DoAction(() =>
                {
                    userForUpdate.FirstName = model.FirstName;
                    userForUpdate.LastName = model.LastName;
                    userForUpdate.Email = model.Email;
                    userForUpdate.Password = model.Password;
                    userForUpdate.Phone = model.Phone;
                    userForUpdate.Photo = model.Photo;
                    userForUpdate.Status = model.Status;

                    m_db.Users.Update(userForUpdate);
                    m_db.SaveChanges();
                });
            }
            return false;
        }

        public bool Delete(int id)
        {
            User user = m_db.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                return DoAction(() =>
                {
                    m_db.Remove(user);
                    m_db.SaveChanges();
                });
            }
            return false;
        }

        public async Task<bool> CreateMultipleUsers(List<UserModel> userModels)
        {
            try
            {
                var newUsers = userModels.Select(u => new User(u));
                await m_db.Users.AddRangeAsync(newUsers);
                await m_db.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
