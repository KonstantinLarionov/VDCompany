using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VDCompanyMVC.Models.DTO;
using VDCompanyMVC.Models.Entitys;
using VDCompanyMVC.Models.Objects;
using VDCompanyMVC.Models.Pages;
using VDCompanyMVC.Models.Secur;

namespace VDCompany.Controllers
{
    public class AdminController : Controller
    {
        #region AUTHLOGIN
        private static readonly StartContext db = new StartContext(new DbContextOptions<StartContext>());
        private (string login, string password) userinfo = (null, null);
        private IQueryable curruser = null;
        public bool Auth()
        {
            try
            {
                var login = HttpContext.Session.GetString("login");
                var password = HttpContext.Session.GetString("password");
                userinfo = (login, password);
                curruser = db.Admins.Where(f => f.Login == login && f.Password == password);
                return db.Admins.Any(f => f.Email == login && f.Password == password);
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToRoute(new { controller = "User", action = "Login" });
        }
        #endregion
        [HttpGet]
        public IActionResult Index()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });

            return RedirectToRoute(new { controller = "Admin", action = "Cases" });
        }



        [HttpGet]
        public IActionResult Contacts()
        {
            /*ContactsDTO contacts = new ContactsDTO
                (
                    HttpContext.SendToUser(u => u.GetLawyers()),
                    HttpContext.SendToUser(u => u.GetContacts())
                );*/
            return View();
        }
        [HttpGet]
        public IActionResult PDN()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Case(int Id)
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            var _case = db.Cases.Where(f => f.Id == Id).FirstOrDefault();
            var user = db.Users.Where(f => f.Login == userinfo.login && f.Password == userinfo.password).FirstOrDefault();
            MyCaseDTO myCaseDTO = new MyCaseDTO
                (
                    _case,
                    user
                );
            return View(myCaseDTO);
        }
        #region BILLS
        [HttpGet]
        public IActionResult Bills()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            var userwithbills = db.Users.Where(f => f.Login == userinfo.login && f.Password == userinfo.password).Include(f => f.Bills).FirstOrDefault();

            var model = new ModelUserBills
            {
                Bills = userwithbills.Bills
            };
            return View(model);
        }
        #endregion

        [HttpGet]
        public IActionResult Cases()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            var allcases = db.Cases.Include(f => f.Lawyers).ToList();
            var alllawyers = db.Lawyers.ToList();
            var model = new ModelAdminCases
            {
                Cases = allcases,
                Lawyers = alllawyers
            };
            return View(model);
        }
        [HttpPost]
        public string AddLawyer(int id_case, int id_lawyer)
        {
            var casewithlawyers = db.Cases.Where(f => f.Id == id_case).Include(d => d.Lawyers).FirstOrDefault();
            var lawyer = db.Lawyers.Where(f => f.Id == id_lawyer).FirstOrDefault();
            if (!casewithlawyers.Lawyers.Any(c => c.Id == id_lawyer))
            {
                casewithlawyers.Lawyers.Add(lawyer);
                db.SaveChanges();
                lawyer.Password = "";
                return "{\"status\":\"success\", \"data\":\"Lawyer id=" + id_lawyer + " success added to case id=" + id_case + "\", \"object\":" + JsonSerializer.Serialize(lawyer) + "}";
            }
            return "{\"status\":\"isset\", \"data\":\"Lawyer id=" + id_lawyer + " already added to case id=" + id_case + "\"}";
        }
        [HttpPost]
        public string GetLawyers(int id_case)
        {
            var caselawyers = db.Cases.Where(f => f.Id == id_case).Include(l => l.Lawyers).FirstOrDefault();
            caselawyers.Lawyers.ForEach(x => { x.Password = ""; });
            var lawyers = db.Lawyers.ToList();
            return JsonSerializer.Serialize(new { CaseLawyers = caselawyers.Lawyers, AllLawyers = lawyers} );
        }
        #region Helpers
        private string GetHash(string data, int length = 0)
        {
            var tmpSource = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
            byte[] tmpNewHash = null;
            string h = "";
            using (var crypto = new System.Security.Cryptography.SHA256CryptoServiceProvider())
            {
                tmpNewHash = crypto.ComputeHash(tmpSource);
            }
            for (int i = 0; i < tmpNewHash.Length; i++)
            {
                h += tmpNewHash[i].ToString("X2");
            }
            return length > 0 ? h.Remove(length) : h;
        }
        private string GenNewPsw(int length, string s = "zaq1xsw2cde3vfr4bgt5nhy6mju7ki8lo9p0")
        {
            string g = "";
            for (int i = 0; i < length; i++)
            {
                g += s[new Random().Next(s.Length)];
            }
            return g;
        }
        #endregion
    }
}
