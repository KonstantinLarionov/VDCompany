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
        private static readonly StartContext db = new StartContext(new DbContextOptions<StartContext>());
        private (string login, string password) userinfo = (null, null);
        private IQueryable curruser = null;
        #region AUTHLOGIN
        public string test(int idCase)
        {
            var userCase = db.Cases.Where(@case => @case.Id == idCase).Include(c => c.Dialog.Messages).FirstOrDefault();
            return "1";
        }
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
        public IActionResult Cases()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            var allcases = db.Cases.ToList();
            var model = new ModelAdminCases
            {
                Cases = allcases
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToRoute(new { controller = "User", action = "Login" });
        }
        #endregion
        #region pages
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
        public IActionResult Users()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            var users = db.Users.ToList();
            return View(new ModelAdminUsers
            { 
                Users = users
            });
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
            var _case = db.Cases.Where(f => f.Id == Id).Include(d => d.Docs).FirstOrDefault();
            var user = db.Users.Where(f => f.Login == userinfo.login && f.Password == userinfo.password).FirstOrDefault();
            var index_lawyers = db.LawyersCases.Where(f => f.CaseId == Id).ToList();
            var lawyers_in_case = new List<Lawyer>();
            foreach (var item in index_lawyers)
            {
                var l = db.Lawyers.Where(f => f.Id == item.LawyerId).FirstOrDefault();
                lawyers_in_case.Add(l);
            }
            MyCaseDTO myCaseDTO = new MyCaseDTO
                (
                    _case,
                    user
                );
            myCaseDTO.Lawyers = lawyers_in_case;
            return View(myCaseDTO);
        }
        [HttpGet]
        public IActionResult Lawyers()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            var lawyers = db.Lawyers.ToList();
            return View(new ModelAdminLawyers { Lawyers = lawyers});
        }
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
        #region forCases
        [HttpPost]
        public string AddLawyer(int id_case, int id_lawyer)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            if (db.Cases.Any(f => f.Id == id_case) && db.Lawyers.Any(f => f.Id == id_lawyer))
            {
                if (!db.LawyersCases.Any(c => c.CaseId == id_case && c.LawyerId == id_lawyer))
                {
                    db.LawyersCases.Add(new LawyersCases(id_case, id_lawyer));
                    db.SaveChanges();
                    var lawyer = db.Lawyers.Where(f => f.Id == id_lawyer).FirstOrDefault();
                    return "{\"status\":\"success\", \"data\":\"Lawyer id=" + id_lawyer + " success added to case id=" + id_case + "\", \"object\":" + JsonSerializer.Serialize(lawyer) + "}";
                }
                return "{\"status\":\"isset\", \"data\":\"Lawyer id=" + id_lawyer + " already added to case id=" + id_case + "\"}";
            }
            return "{\"status\":\"error \"data\":\"Lawyer id=" + id_lawyer + " or case id=" + id_case + " not found\"}";
        }
        [HttpPost]
        public string DelLawyer(int id_case, int id_lawyer)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var l = db.LawyersCases.Where(f => f.CaseId == id_case && f.LawyerId == id_lawyer).FirstOrDefault();
            if (l != null)
            {
                db.LawyersCases.Remove(l);
                db.SaveChanges();
                return "{\"status\":\"success\", \"data\":\"Lawyer id=" + id_lawyer + " was success deleted from case id=" + id_case + "\"}";
            }
            return "{\"status\":\"error\", \"data\":\"Lawyer id=" + id_lawyer + " not found case id=" + id_case + "\"}";
        }
        [HttpPost]
        public string GetLawyers(int id_case)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var caselawyers = db.LawyersCases.Where(f => f.CaseId == id_case).ToList();
            List<Lawyer> lawyers = new List<Lawyer>();
            foreach (var item in caselawyers)
            {
                var lawyer = db.Lawyers.Where(f => f.Id == item.LawyerId).FirstOrDefault();
                if (lawyer != null)
                {
                    lawyers.Add(lawyer);
                }
            }
            var alllawyers = db.Lawyers.ToList();
            return JsonSerializer.Serialize(new { status = "success", CaseLawyers = lawyers, AllLawyers = alllawyers } );
        }
        #endregion
        #region forLawyers
        [HttpPost]
        public string NewLawyer(string fio, string login, string password)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            db.Lawyers.Add(new Lawyer
            { 
                DateCreate = DateTime.Now,
                FIO = fio,
                Login = login,
                Password = password
            });
            db.SaveChanges();
            var new_id = db.Lawyers.Select(f => f.Id).Max(); 
            return "{\"status\":\"success\", \"data\":\"success added new lawyer\", \"id\":" + new_id + "}";
        }
        [HttpPost]
        public string EditLawyer(int id, string fio, string login, string password)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var lawyer = db.Lawyers.Where(f => f.Id == id).FirstOrDefault();
            if (lawyer != null)
            {
                lawyer.FIO = fio;
                lawyer.Login = login;
                lawyer.Password = password;
                db.SaveChanges();
                return "{\"status\":\"success\", \"data\":\"success edit lawyer with id = " + id + "\"}";
            }
            return "{\"status\":\"error\", \"data\":\"not found lawyer with id = " + id + "\"}";
        }
        [HttpPost]
        public string RemoveLawyer(int id)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var lawyer = db.Lawyers.Where(f => f.Id == id).FirstOrDefault();
            if (lawyer != null)
            {
                db.Lawyers.Remove(lawyer);
                var lawyers_from_cases = db.LawyersCases.Where(f => f.LawyerId == id).ToList();
                if(lawyers_from_cases.Count > 0)
                    db.LawyersCases.RemoveRange(lawyers_from_cases);
                db.SaveChanges();
                return "{\"status\":\"success\", \"data\":\"success remove lawyer with id = " + id + "\"}";
            }
            return "{\"status\":\"error\", \"data\":\"not found lawyer with id = " + id + "\"}";
        }
        [HttpPost]
        public string GetInfoLawyer(int id)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var lawyer = db.Lawyers.Where(f => f.Id == id).FirstOrDefault();
            if (lawyer != null)
            {
                return "{\"status\":\"success\", \"data\":" + JsonSerializer.Serialize(lawyer) + "}";
            }
            return "{\"status\":\"error\", \"data\":\"not found lawyer with id = " + id + "\"}";
        }
        #endregion
        #region forUsers
        [HttpPost]
        public string EditUser(int id, string fio, string login, string password)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var user = db.Users.Where(f => f.Id == id).FirstOrDefault();
            if (user != null)
            {
                user.Name = fio;
                user.Login = login;
                user.Password = password;
                db.SaveChanges();
                return "{\"status\":\"success\", \"data\":\"success edit user with id = " + id + "\"}";
            }
            return "{\"status\":\"error\", \"data\":\"not found user with id = " + id + "\"}";
        }
        [HttpPost]
        public string GetInfoUser(int id)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var user = db.Users.Where(f => f.Id == id).FirstOrDefault();
            if (user != null)
            {
                return "{\"status\":\"success\", \"data\":" + JsonSerializer.Serialize(user) + "}";
            }
            return "{\"status\":\"error\", \"data\":\"not found user with id = " + id + "\"}";
        }
        [HttpPost]
        public string GetUserBills(int id)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var user = db.Users.Where(f => f.Id == id).Include(g => g.Bills).FirstOrDefault();
            if (user != null)
            {
                return "{\"status\":\"success\", \"data\":" + JsonSerializer.Serialize(user.Bills) + "}";
            }
            return "{\"status\":\"error\", \"data\":\"not found user with id = " + id + "\"}";
        }
        [HttpPost]
        public string GetUserCases(int id)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var user = db.Users.Where(f => f.Id == id).Include(g => g.Cases).FirstOrDefault();
            if (user != null)
            {
                return "{\"status\":\"success\", \"data\":" + JsonSerializer.Serialize(user.Cases) + "}";
            }
            return "{\"status\":\"error\", \"data\":\"not found user with id = " + id + "\"}";
        }
        [HttpPost]
        public string AddUserBill(int id, string name, string amount, string req)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var userwithbills = db.Users.Where(f => f.Id == id).Include(b => b.Bills).FirstOrDefault();
            if (userwithbills != null)
            {
                userwithbills.Bills.Add(new Bill
                { 
                    NameCase = name,
                    Amount = Convert.ToDouble(amount.Replace('.', ',')),
                    Requizit = req,
                    Status = StatusBill.InProcess,
                    DateCreate = DateTime.Now
                });
                db.SaveChanges();
                var new_id = db.Users.Where(f => f.Id == id).Include(b => b.Bills).FirstOrDefault().Bills.Select(f => f.Id).Max();
                return "{\"status\":\"success\", \"data\":\"success added new bill id = " + new_id + "\", \"id\":" + new_id + "}";
            }
            return "{\"status\":\"error\", \"data\":\"not found user with id = " + id + "\"}";
        }
        [HttpPost]
        public string UserBillChangeStatus(int user_id, int bill_id, string status)
        {
            if (!Auth())
                return "{\"status\":\"not_authorized\"}";
            var user = db.Users.Where(f => f.Id == user_id).Include(b => b.Bills).FirstOrDefault();
            if (user != null)
            {
                if (user.Bills.Any(f => f.Id == bill_id))
                {
                    var bill = user.Bills.Where(f => f.Id == bill_id).FirstOrDefault();
                    bill.DatePay = DateTime.Now;
                    if (status == "payed")
                        bill.Status = StatusBill.Paid;
                    if (status == "cancel")
                        bill.Status = StatusBill.NotPaid;
                    if (status == "inprocess")
                        bill.Status = StatusBill.InProcess;
                    db.SaveChanges();
                    return "{\"status\":\"success\", \"data\":\"success change status bill id = " + bill_id+ "\"}";
                }
            }
            return "{\"status\":\"error\", \"data\":\"not found user with id = " + bill_id + "\"}";
        }
        #endregion
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
