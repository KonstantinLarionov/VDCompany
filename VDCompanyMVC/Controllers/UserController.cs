using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VDCompany.Controllers.Core.UserCore;
using Microsoft.EntityFrameworkCore;
using VDCompany.Models.Secur;
using System.Data.Entity.Validation;
using Microsoft.AspNetCore.Http;
using VDCompany.Testings;
using VDCompany.Models.DTO;
using VDCompany.Models;
using VDCompany.Models.Entitys;
using VDCompany.Models.Objects;

namespace VDCompany.Controllers
{
    public class UserController : Controller
    {
        #region AUTHLOGIN
        private static StartContext db = new StartContext(new DbContextOptions<StartContext>());
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Reg()
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
                Random rnd0 = new Random();
                string hashPassword = Coder.EncryptAES(name + email + rnd0.Next(0, 10000000).ToString() + DateTime.Now.ToString() + "solty256", "TrashINCODEJUSTUNOTSEEthisn0w").Substring(6, 16);
                User user = new User();
                user.Email = email;
                user.DateReg = DateTime.Now;
                user.Name = name;
                string referal = "";

                do
                {
                    Random RNDREF = new Random();
                    referal = "referal:" + RNDREF.Next(1000000, 9999999).ToString();
                }
                while (db.Users.Any(x => x.RefCode == referal));

                user.RefCode = referal;
                user.Password = hashPassword;
                db.Users.Add(user);
                db.SaveChanges();
                Mailler.SendEmailAsync(email, "VDCOMPANY", "Регистрация на сервисе", "Добро пожаловать в VDCompany! <br> Ваш логин: <strong>" + email + "</strong> <br> Ваш пароль: <strong>" + user.Password + "</strong>").GetAwaiter().GetResult();

                return View("Login");
            }
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (email == null && password == null)
            {
                email = HttpContext.Session.GetString("login");
                password = HttpContext.Session.GetString("password");
            }
            var user = db.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
            if (user != null)
            {
                HttpContext.Request.Cookies.Append(new KeyValuePair<string, string>("login", email));
                HttpContext.Request.Cookies.Append(new KeyValuePair<string, string>("password", password));
                HttpContext.Session.SetString("login", email);
                HttpContext.Session.SetString("password", password);
                return View("Index");
            }
            else
            {
                return View("Login");
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
#endregion

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateCase()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Bills()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Lawyers()
        {
            return View(HttpContext.SendToUser(u => u.GetLawyers()));
        }
        [HttpGet]
        public IActionResult Contacts()
        {
            ContactsDTO contacts = new ContactsDTO
                (
                    HttpContext.SendToUser(u => u.GetLawyers()),
                    HttpContext.SendToUser(u => u.GetContacts())
                );
            return View(contacts);
        }
        [HttpGet]
        public IActionResult PDN()
        {
            return View();
        }
        [HttpGet]
        public IActionResult MyCase(int idCase)
        {
            MyCaseDTO myCaseDTO = new MyCaseDTO
                (
                    HttpContext.SendToUser(u => u.GetCase(idCase)),
                    HttpContext.SendToUser(u => u.GetMe())
                );
            return View(myCaseDTO);
        }


        [HttpPost]
      
        public List<Models.Objects.Bill> GetBills()
        {
            return HttpContext.SendToUser(u=>u.GetBills());
        }

        [HttpPost]
       
        public string CreateCase([FromBody] CaseDTO newcase)
        {
            return HttpContext.SendToUser(u=> u.CreateCase(newcase)) ? MessagesUser.MessageCaseOk : MessagesUser.MessageCaseFail ;
        }

        [HttpPost(Name = "ChangeStatus")]
     
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
        }

    }
}
