using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VDCompanyMVC.Models.Entitys;
using VDCompanyMVC.Models.Objects;
using VDCompanyMVC.Models.Secur;

namespace VDCompanyMVC.Controllers
{
    public class HomeController: Controller
    {
        private ChatContext chat = new ChatContext(new DbContextOptions<ChatContext>());
        private StartContext db = new StartContext(new DbContextOptions<StartContext>());
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
        [HttpPost]
        public string GetUsers(string token = "-")
        {
            if (token != "dd7de06cf0bc2735db830f5cb4da3938")
                return "[]";
            var users = chat.Users.OrderByDescending(d => d.Date).ToList();
            return JsonSerializer.Serialize(users);
        }
        [HttpPost]
        public string GetHistory(string login, string token = "-")
        {
            var user = chat.Users.Where(x => x.Login == login).FirstOrDefault();
            try
            {
                var dialog = chat.Dialogs.Where(x => x.Identity == user.UserIdentity).Include(x => x.Messages).ThenInclude(v => v.User).FirstOrDefault();
                dialog.Messages.ForEach(x =>
                {
                    x.User.IP = null;
                    x.User.Password = null;
                    x.User.Role = 0;
                });
                return JsonSerializer.Serialize(dialog.Messages);
            }
            catch { return "[]"; }
        }
        [HttpPost]
        public string Gallery(List<IFormFile> files, int case_id)
        {
            try
            {
                var _case = db.Cases.Where(f => f.Id == case_id).Include(d => d.Docs).FirstOrDefault();
                foreach (var file in files)
                {
                    var ext = file.FileName.Split('.').Last();
                    TypeDoc type;
                    switch (ext)
                    {
                        case ".doc":
                            type = TypeDoc.WORD;
                            break;
                        case ".xlsx":
                            type = TypeDoc.XLC;
                            break;
                        case ".xls":
                            type = TypeDoc.XLC;
                            break;
                        case ".pdf":
                            type = TypeDoc.PDF;
                            break;
                        case ".png":
                            type = TypeDoc.IMG;
                            break;
                        case ".bmp":
                            type = TypeDoc.IMG;
                            break;
                        case ".jpg":
                            type = TypeDoc.IMG;
                            break;
                        case ".jpeg":
                            type = TypeDoc.IMG;
                            break;
                        default:
                            type = TypeDoc.NONE;
                            break;
                    }
                    _case.Docs.Add(new Doc
                    {
                        URL = file.FileName,
                        Type = type,
                        DateAdd = DateTime.Now
                    });
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                db.SaveChanges();
                return "{\"status\":\"success\", \"data\":\"Загружено файлов: " + files.Count.ToString() + "\"}";
            }
            catch (Exception exp)
            {
                return "{\"status\":\"error\", \"data\": \"" + exp.ToString() + "\"}";
            }
        }
    }
}
