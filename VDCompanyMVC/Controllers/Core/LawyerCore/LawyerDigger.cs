using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Entitys;
using VDCompany.Models.Objects;
using VDCompanyMVC;

namespace VDCompany.Controllers.Core.LawyerCore
{
    public class LawyerDigger
    {
        #region PrivateProps
        private Action<string> Logs => Startup.Logs;
        private static HttpContext http;
        private StartContext db;
        #endregion
        #region PublicProps
        public LawyerDigger(object arg)
        {
            http = arg as HttpContext;
            db = new StartContext(new DbContextOptions<StartContext>());
        }
        #endregion
        #region LawTaker
        public bool Auth(string login = "", string password = "")
        {
            if (login == "" && password == "")
            {
                login = http.Session.GetString("login");
                password = http.Session.GetString("password");
            }
            var user = db.Lawyers.Where(u => u.Login == login && u.Password == password).ToList();
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
        public IQueryable<Lawyer> GetLaw()
        {
            return db.Lawyers.Where(u => u.Email == http.Session.GetString("login") &&
            u.Password == http.Session.GetString("password"));
        }
        #endregion
        #region PublicInfoTaker
        public List<User> GetUsersLaw()
        {
            var law = GetLaw().Include(x => x.Users).FirstOrDefault();
            return law.Users;
        }
        public List<Case> GetCasesLaw()
        {
            var law = GetLaw().FirstOrDefault();
            var lawCases = db.Cases.Where(x => x.Lawyers.Any(x => x.Id == law.Id)).ToList();
            return lawCases;
        }
        public Case GetCase(int id)
        {
            var @case = db.Cases
                .Where(x => x.Id == id)
                .Include(x => x.Lawyers)
                .Include(x => x.ClientsHub)
                .Include(x => x.Dialog)
                .Include(x => x.Docs)
                .FirstOrDefault();
            return @case;
        }
        public Lawyer GetMyInfo()
        {
            var law = GetLaw().Include(x=>x.Image).Include(x=>x.Users).FirstOrDefault();
            return law;
        }
        #endregion
        #region Helpers
        private void Save() => db.SaveChanges();

        #endregion
    }
}
