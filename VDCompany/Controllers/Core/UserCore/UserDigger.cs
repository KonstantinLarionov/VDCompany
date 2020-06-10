using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Entitys;
using VDCompany.Models.Objects;
using Microsoft.AspNetCore.Http;
using VDCompany.Testings;
using System.IO;
using VDCompany.Models.DTO;

namespace VDCompany.Controllers.Core.UserCore
{
    public enum ResultState
    { 
        None, Ok, Fail, Waiting, NotAuth
    }
    public class UserDigger
    {
        #region PrivateProps
        private Action<string> Logs => Startup.Logs;
        private static HttpContext http;
        private StartContext db;
        #endregion
        #region PublicProps
        public UserDigger(object arg)
        {
            http = arg as HttpContext;
            db = new StartContext(new DbContextOptions<StartContext>());
        }
        #endregion
        #region UserTaker
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
        public IQueryable<User> GetUser()
        {
            return db.Users.Where(u => u.Email == http.Session.GetString("login") &&
            u.Password == http.Session.GetString("password"));
        }
        #endregion
        #region UserInfoTaker
        public List<Bill> GetBills()
        {
            /*if (Auth())
            {*/
            UserDBBuilder.HttpContext = http;
            UserDBBuilder.Build("cookies");

            var userWithBills = GetUser().Include(x => x.Bills).FirstOrDefault();
            return userWithBills.Bills;
            /*}
            else 
            {
                return null;
            }*/
        }
        #endregion
        #region UserInfoSetter
        public ResultState ChangeStatusBill(int id)
        {
            try
            {
                var user = GetUser().Include(x => x.Bills).FirstOrDefault();
                user.Bills.Where(x => x.Id == id).FirstOrDefault().Status = StatusBill.InProcess;
                Save();
                return ResultState.Ok;
            }
            catch (Exception ex)
            {
                Logs?.Invoke(ex.ToString());
                return ResultState.Fail;
            }
        }
        public ResultState CreateCase(CaseDTO caseDTO)
        {
            try
            {
                UserDBBuilder.HttpContext = http;
                UserDBBuilder.Build("user");
                UserDBBuilder.Build("cookies");
                Case new_case = new Case();
                new_case.Name = caseDTO.Name;
                new_case.Type = caseDTO.Type;
                new_case.Description = caseDTO.Description;
                new_case.DateStart = DateTime.Now;
                var user = GetUser().Include(x=>x.Cases).FirstOrDefault();
                user.Cases.Add(new_case);
                Save();
                return ResultState.Ok;
            }
            catch(Exception ex)
            {
                Logs?.Invoke(ex.ToString());
                return ResultState.Fail;
            }
        }
        #endregion
        #region Helpers
        private void Save() => db.SaveChanges();
       
        #endregion
    }
}
