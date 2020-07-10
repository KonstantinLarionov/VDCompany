using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Entitys;
using VDCompany.Models.Objects;

namespace VDCompany.Controllers.Core.AdminCore
{
    public class AdminDigger
    {
        #region PrivateProps
        private Action<string> Logs => Startup.Logs;
        private static HttpContext http;
        private StartContext db;
        #endregion
        #region PublicBase
        public AdminDigger(object arg)
        {
            http = arg as HttpContext;
            db = new StartContext(new DbContextOptions<StartContext>());
        }

        public bool Auth(string login = "", string password = "")
        {
            if (login == "" && password == "")
            {
                login = http.Session.GetString("login");
                password = http.Session.GetString("password");
            }
            var user = db.Admins.Where(u => u.Login == login && u.Password == password).ToList();
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
        public IQueryable<Admin> GetAdmin()
        {
            return db.Admins.Where(u => u.Email == http.Session.GetString("login") &&
            u.Password == http.Session.GetString("password"));
        }
        #endregion
        #region PublicGetterInfo
        public List<User> GetUsers()
        {
            var users = db.Users.ToList();
            return users;
        }
        public List<Lawyer> GetLawyers()
        {
            var lawyers = db.Lawyers.ToList();
            return lawyers;
        }
        public User GetUser(int id)
        {
            var user = db.Users.Where(x => x.Id == id).FirstOrDefault();
            return user;
        }
        public Lawyer GetLawyer(int id)
        {
            var law = db.Lawyers.Where(x => x.Id == id).FirstOrDefault();
            return law;
        }
        public List<Case> GetCases()
        {
            var cases = db.Cases.Include(x=>x.Lawyers).ToList();
            return cases;
        }
        public Case GetCase(int id)
        {
            var @case = db.Cases.Where(x=>x.Id == id)
                .Include(x=>x.Lawyers)
                .Include(x=>x.Docs)
                .Include(x=>x.ClientsHub)
                .Include(x=>x.Dialog).FirstOrDefault();
            return @case;
        }
        public Dialog GetDialog(int id)
        {
            var dialog = db.Dialogs.Where(x => x.Id == id).FirstOrDefault();
            return dialog; 
        }
        #endregion
        #region PublicSetterInfo
        public void SetLawyer(Lawyer lawyer)
        {
            db.Lawyers.Add(lawyer);
            Save();
        }
        public void SetUser(User user)
        {
            db.Users.Add(user);
            Save();
        }
        public void ChangeStatusCase(int idCase, TypeCase type)
        {
            var @case = db.Cases.Where(x => x.Id == idCase).FirstOrDefault();
            @case.Status = type;
            Save();
        }

        public void DeleteLawyer(int id)
        {
            var law = db.Lawyers.Where(x => x.Id == id).FirstOrDefault();
            db.Lawyers.Remove(law);
            Save();
        }

        public void DeleteUser(int id)
        {
            var user = db.Users.Where(x => x.Id == id).FirstOrDefault();
            db.Users.Remove(user);
            Save();
        }

        public void DeleteCase(int id)
        {
            var @case = db.Cases.Where(x => x.Id == id).FirstOrDefault();
            db.Cases.Remove(@case);
            Save();
        }
        #endregion
        #region Helpers
        private void Save() => db.SaveChanges();
        #endregion
    }
}
