using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompanyMVC.Models.Entitys;
using VDCompanyMVC.Models.Secur;

namespace VDCompanyMVC.Controllers
{
    public class HomeController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public string SendFeedBack(string name, string email, string message)
        {
            try
            {
                Mailler.SendEmailAsync("mr.mishana-319@yandex.ru", "VDCOMPANY", "Обратная связь",
                $"Новое собщение через обратную связь <br> <strong>Имя:</strong> { name } <br> <strong>Обратный Email:</strong> { email } <br> <strong>Сообщение:</strong><br>{ message }"
                ).GetAwaiter().GetResult();
                return "{ \"status\": \"success\" }";
            }
            catch
            {
                return "{ \"status\": \"error\" }";
            }
        }
    }
}
