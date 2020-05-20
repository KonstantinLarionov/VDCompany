using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VDCompany.Models.DTO;
using VDCompany.Models.Entitys;
using VDCompany.Models.Objects;
using VDCompany.Models.Secur;

namespace VDCompany.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static StartContext db = new StartContext(new DbContextOptions<StartContext>());

        [HttpPost(Name = "Reg")]
        [Route("Reg")]
        public string Reg([FromBody] RegDTO userDTO)
        {
            if (db.Users.Any(x => x.Email == userDTO.Email))
            {
                return "Данный Email был зарегистрирован ранее. Попробуйте восстановить пароль.";
            }
            else
            {
                Random rnd0 = new Random();
                string hashPassword = Coder.EncryptAES(userDTO.Name + userDTO.Email + rnd0.Next(0, 10000000).ToString() + DateTime.Now.ToString() + "solty256", "TrashINCODEJUSTUNOTSEEthisn0w").Substring(6, 16);
                User user = new User();
                user.Email = userDTO.Email;
                user.DateReg = DateTime.Now;
                user.Name = userDTO.Name;
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
                Mailler.SendEmailAsync(userDTO.Email, "VDCOMPANY", "Регистрация на сервисе", "Добро пожаловать в VDCompany! <br> Ваш логин: <strong>" + userDTO.Email + "</strong> <br> Ваш пароль: <strong>" + user.Password + "</strong>").GetAwaiter().GetResult();

                return "Пользователь успешно зарегистрирован на сервисе. Проверьте вашу почту";
            }
        }

        [HttpPost(Name = "Login")]
        [Route("Login")]
        public bool Login([FromBody] LoginDTO loginDTO = null)
        {
            if (loginDTO == null)
            {
                loginDTO.Login = HttpContext.Session.GetString("login");
                loginDTO.Password = HttpContext.Session.GetString("password");
            }
            var user = db.Users.Where(u => u.Email == loginDTO.Login && u.Password == loginDTO.Password).FirstOrDefault();
            if (user != null)
            {
                HttpContext.Request.Cookies.Append(new KeyValuePair<string, string>("login", loginDTO.Login));
                HttpContext.Request.Cookies.Append(new KeyValuePair<string, string>("password", loginDTO.Password));
                HttpContext.Session.SetString("login", loginDTO.Login);
                HttpContext.Session.SetString("password", loginDTO.Password);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
