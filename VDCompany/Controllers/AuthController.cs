using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public string Reg([FromForm] RegDTO userDTO)
        {
            if (db.Users.Any(x => x.Email == userDTO.Email))
            {
                return "Данный Email был зарегистрирован ранее. Попробуйте восстановить пароль.";
            }
            else
            {
                Coder coder = new Coder("letsHashEntropy256");
                string hashPassword = coder.Encrypt(DateTime.Now.ToString()+ "solty256" + userDTO.Email + userDTO.Name).Skip(2).Take(6).ToString();
                string hasRef = coder.Encrypt("referal:" + DateTime.Now.ToString()).Take(6).ToString();
                User user = new User();
                user.Email = userDTO.Email;
                user.DateReg = DateTime.Now;
                user.Name = userDTO.Name;
                user.RefCode = "referal-" + hasRef;
                user.Password = hashPassword;
                db.Users.Add(user);
                db.SaveChanges();
                return "Пользователь успешно зарегистрирован на сервисе. Проверьте вашу почту";
            }
        }
    }
}
