using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Data.Entity.Validation;
using Microsoft.AspNetCore.Http;

using VDCompanyMVC.Models.DTO;
using VDCompanyMVC.Models.Pages;
using VDCompanyMVC.Models.Entitys;
using VDCompanyMVC.Models.Objects;
using VDCompanyMVC.Models.Secur;

namespace VDCompany.Controllers
{
    public class UserController : Controller
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
                curruser = db.Users.Where(f => f.Login == login && f.Password == password);
                return db.Users.Any(f => f.Email == login && f.Password == password);
            }
            catch
            {
                return false;
            }
        }
        public IActionResult Reg()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Reg(string email, string name)
        {
            if (db.Users.Any(x => x.Email == email))
            {
                return View("Reg");
            }
            else
            {
                string referal = "";
                string psw = GenNewPsw(10);
                string message =  $"Добро пожаловать в VDCompany! <br> Ваш логин: <strong> { email } </strong> <br> Ваш пароль: <strong> { psw } </strong>";
                Mailler.SendEmailAsync(email, "VDCOMPANY", "Регистрация на сервисе", message).GetAwaiter().GetResult();
                do
                {
                    Random RNDREF = new Random();
                    referal = RNDREF.Next(1000000, 9999999).ToString();
                }
                while (db.Users.Any(x => x.RefCode == referal));
                db.Users.Add(new User
                { 
                    Email = email,
                    Login = email,
                    Name = name,
                    DateReg = DateTime.Now,
                    RefCode = $"referal:{ referal }",
                    Password = psw
                });
                db.SaveChanges();
                return Redirect("Login");
            }
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            User user = null;
            Admin admin = null;
            Lawyer lawyer = null;

            user = db.Users.Where(u => u.Login == email && u.Password == password).FirstOrDefault();
            if (user != null)
            {
                HttpContext.Session.SetString("login", email);
                HttpContext.Session.SetString("password", password);
                HttpContext.Response.Cookies.Append("login", email);
                HttpContext.Response.Cookies.Append("password", password);
                return RedirectToRoute(new { controller = "User", action = "Cases" });
            }
            else 
            {
                lawyer = db.Lawyers.Where(u => u.Login == email && u.Password == password).FirstOrDefault();
                if (lawyer != null)
                {
                    HttpContext.Session.SetString("login", email);
                    HttpContext.Session.SetString("password", password);
                    HttpContext.Response.Cookies.Append("login", email);
                    HttpContext.Response.Cookies.Append("password", password);
                    return RedirectToRoute(new { controller = "Lawyer", action = "Index" });
                }
                else 
                {
                    admin = db.Admins.Where(u => u.Login == email && u.Password == password).FirstOrDefault();
                    if (admin != null)
                    {
                        HttpContext.Session.SetString("login", email);
                        HttpContext.Session.SetString("password", password);
                        HttpContext.Response.Cookies.Append("login", email);
                        HttpContext.Response.Cookies.Append("password", password);
                        return RedirectToRoute(new { controller = "Admin", action = "Index" });
                    }
                    else 
                    {
                        return RedirectToRoute(new { controller = "User", action = "Login" });
                    }
                }
            }            
        }
        [HttpPost]
        public string Reset([FromBody] RegDTO userDTO)
        {
            if (db.Users.Any(x => x.Email == userDTO.Email))
            {
                var user = db.Users.Where(x => x.Email == userDTO.Email).FirstOrDefault();
                Mailler.SendEmailAsync(userDTO.Email, "VDCOMPANY", "Забыли пароль?", "Ваш пароль на сервисе VDCompany: <strong>" + user.Password + "</strong>").GetAwaiter().GetResult();

                return "Пароль успешно восстановлен. Проверьте вашу почту";
            }
            else
            {
                return "Данный Email не был зарегистрирован ранее. Пройдите процедуру регистрации.";
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
            if(!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            
            return RedirectToRoute(new {controller = "User", action = "Cases"} );
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
        #region CASES
        [HttpGet]
        public IActionResult CreateCase()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            return View();
        }
        [HttpPost]
        public string CreateCase([FromBody] CaseDTO newcase)
        {
            if (!Auth())
                return "{\"info\":\"unauthorized\"}";
            var userwithcases = db.Users.Where(f => f.Login == userinfo.login && f.Password == userinfo.password).Include(x => x.Cases).FirstOrDefault();
            Case new_case = new Case();
            try
            {
                new_case.Name = newcase.Name;
                new_case.Type = newcase.Type;
                new_case.Dialog = new Dialog() { DateCreate = DateTime.Now, Admins = new List<Admin>(), Lawyers = new List<Lawyer>(), Messages = new List<Message>(), Users = new List<User>() { userwithcases } };
                new_case.Description = newcase.Description;
                new_case.DateStart = DateTime.Now;
                userwithcases.Cases.Add(new_case);
                db.SaveChanges();
            }
            catch
            {
                return "{\"info\":\"error\"}";
            }
            try
            {
                Mailler.SendEmailAsync(userinfo.login, "VDCOMPANY", "Создание нового дела",
                    $"Вы создали новое дело на сервисе VDCOMPANY! <br><br> Наименование вашего дела: {new_case.Name} <br> Тип вашего дела: {new_case.Type} <br> Дата создания: {new_case.DateStart} <br><br> После регистрации, дело появится в вашем личном кабинете в списке дел и вам будет назначен подходящий специалист.<br><br> <span style=\"color:red;\">По всем вопросам: companyvd@yandex.ru</span>").GetAwaiter().GetResult();
            }
            catch 
            {
            
            }
                return "{\"info\":\"success\"}";
        }
        [HttpGet]
        public IActionResult Cases()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            var userwithcases = db.Users.Where(f => f.Email == userinfo.login && f.Password == userinfo.password).Include(x => x.Cases).FirstOrDefault();
            var model = new ModelUserCases
            {
                Cases = userwithcases.Cases
            };
            return View(model);
        }
        #endregion
        /*[HttpPost(Name = "ChangeStatus")]
        public object[] ChangeStatus([FromBody] ChangerBillDTO CBD )
        {
            return new object[] { HttpContext.SendToUser(u => u.ChangeStateBill(CBD.Id)), CBD.Id };
        }
        [HttpPost(Name = "GetCases")]
        public List<Models.Objects.Case> GetCases()
        {
            return HttpContext.SendToUser(u => u.GetCases());
        }
        [HttpPost(Name = "GetLawyers")]
        public List<Models.Objects.Lawyer> GetLawyers()
        {
            return HttpContext.SendToUser(u => u.GetLawyers());
        }*/
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
