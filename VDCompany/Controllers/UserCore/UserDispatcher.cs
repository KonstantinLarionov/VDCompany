using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Entitys;
using VDCompany.Models.Objects;
using Microsoft.AspNetCore.Http;

namespace VDCompany.Controllers.UserCore
{
    public class UserDigger
    {
        private static HttpContext http;
        private StartContext db;

        public UserDigger(object arg)
        {
            http = arg as HttpContext;
            db = new StartContext(new DbContextOptions<StartContext>());
        }

        public IQueryable<User> GetUser()
        {
            return db.Users.Where(u => u.Email == http.Session.GetString("login") &&
            u.Password == http.Session.GetString("password"));
        }

        public bool Auth(string login = "", string password = "")
        {
            if (login == "" && password == "")
            {
                login = http.Session.GetString("login");
                password = http.Session.GetString("password");
            }
            var user = db.Users.Where(u => u.Login == login && u.Password == password).ToList();
            if (user.Count != 0)
            {
                http.Request.Cookies.Append(new KeyValuePair<string, string>("login", login));
                http.Request.Cookies.Append(new KeyValuePair<string, string>("password", password));
                http.Session.SetString("login", login);
                http.Session.SetString("password", password);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
