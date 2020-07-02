using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using VDCompany.Models.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using VDCompany.Models.Objects;
using System.IO;

namespace VDCompany.Hubs
{
    public class CaseHub : Hub
    {
        private StartContext database = new StartContext(new DbContextOptions<StartContext>());
        public async Task TriggerHandle(string login, string password, int idCase, string message, List<IFormFile> files = null)
        {
            #region TakeAuth
            var userRes = AuthUser(login, password);
            var adminRes = AuthAdmin(login, password);
            var lawyerRes = AuthLawyer(login, password);
            #endregion
            #region IfUserAuth
            //Аутентификация пользователя
            if (userRes.Item2)
            {
                var user = userRes.Item1;
                var userCase = database.Cases.Where(@case => @case.Id == idCase).FirstOrDefault();
                //Наличие дела у пользователя
                if (userCase != null)
                {
                    //Если подключенного пользователя нет в списке пиров по делу добавляем его в список
                    AddHubConnectToCase(userCase);
                    string urlImage = string.Empty;
                    urlImage = LoadingFileReturnFirstImage(files, userCase);
                    if (message != string.Empty)
                    {
                        userCase.Dialog.Messages.Add(new Message() { Date = DateTime.Now, Text = message, User = user });
                    }
                    var clientsHub = userCase.ClientsHub.Select(ch => ch.ConnectionId).ToList();
                    database.SaveChanges();
                    await Clients.Clients(clientsHub)
                        .SendAsync("TriggerHandle", JsonSerializer.Serialize(new TriggerHandleDTO(user, null, null, message, urlImage, DateTime.Now)));
                }
            }
            #endregion
            #region IfAdminAuth
            else if (adminRes.Item2)
            {
                var admin = adminRes.Item1;
            }
            #endregion
            #region IfLawyerAuth
            else if (lawyerRes.Item2)
            {
                var lawyer = lawyerRes.Item1;
            }
            #endregion
            else { return; }
        }

        private static string LoadingFileReturnFirstImage(List<IFormFile> files, Case userCase)
        {
            string urlImage = string.Empty;
            if (files != null && files.Count != 0)
            {
                files.ForEach(file =>
                {
                    TypeDoc typeFile = TypeFile(file);
                    if (typeFile == TypeDoc.IMG)
                    {
                        urlImage = file.FileName;
                    }
                    userCase.Docs.Add(new Doc() { DateAdd = DateTime.Now, URL = file.FileName, Type = typeFile });
                });
            }

            return urlImage;
        }



        #region Helpers
        private static TypeDoc TypeFile(IFormFile file)
        {
            TypeDoc typeFile;
            switch (file.FileName.Split('.')[1])
            {
                case "mp3":
                    typeFile = TypeDoc.AUDIO;
                    break;
                case "png":
                    typeFile = TypeDoc.IMG;
                    break;
                case "jpg":
                    typeFile = TypeDoc.IMG;
                    break;
                case "jpeg":
                    typeFile = TypeDoc.IMG;
                    break;
                case "bmp":
                    typeFile = TypeDoc.IMG;
                    break;
                case "gif":
                    typeFile = TypeDoc.IMG;
                    break;
                case "ico":
                    typeFile = TypeDoc.IMG;
                    break;
                case "raw":
                    typeFile = TypeDoc.IMG;
                    break;
                case "svg":
                    typeFile = TypeDoc.IMG;
                    break;
                case "pdf":
                    typeFile = TypeDoc.PDF;
                    break;
                case "doc":
                    typeFile = TypeDoc.WORD;
                    break;
                case "docx":
                    typeFile = TypeDoc.WORD;
                    break;
                case "xlc":
                    typeFile = TypeDoc.XLC;
                    break;
                case "xlcx":
                    typeFile = TypeDoc.XLC;
                    break;
                case "3gp":
                    typeFile = TypeDoc.VIDEO;
                    break;
                case "mp4":
                    typeFile = TypeDoc.VIDEO;
                    break;
                case "wmv":
                    typeFile = TypeDoc.VIDEO;
                    break;
                default:
                    typeFile = TypeDoc.NONE;
                    break;
            }
            return typeFile;
        }
        private static void LoadFile(List<IFormFile> images, int id)
        {
            if (images != null && images.Count() != 0)
            {
                foreach (var file in images)
                {
                    if (file.Length <= 10485760)
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\case\\case_" + id.ToString(), file.FileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                }
            }
            
        }
        private (User?, bool) AuthUser(string login, string password)
        {
            var user = database.Users.Where(u => u.Email == login && u.Password == password).FirstOrDefault();
            return user == null ? (null, false) : (user, true); 
        }
        private (Admin?, bool) AuthAdmin(string login, string password)
        {
            var admin = database.Admins.Where(u => u.Email == login && u.Password == password).FirstOrDefault();
            return admin == null ? (null, false) : (admin, true);
        }
        private (Lawyer?, bool) AuthLawyer(string login, string password)
        {
            var lawyer = database.Lawyers.Where(u => u.Email == login && u.Password == password).FirstOrDefault();
            return lawyer == null ? (null, false) : (lawyer, true);
        }
        private void AddHubConnectToCase(Case userCase)
        {
            if (userCase.ClientsHub == null || userCase.ClientsHub.Any(ch => ch.ConnectionId != Context.UserIdentifier))
            {
                userCase.ClientsHub.Add(new ClientHub() { ConnectionId = Context.UserIdentifier, LastNotify = DateTime.Now });
            }
        }
        #endregion
    }
    public class TriggerHandleDTO
    {
        public TriggerHandleDTO(User user, Admin admin, Lawyer lawyer, string message, string image, DateTime date)
        {
            User = user;
            Admin = admin;
            Lawyer = lawyer;
            Message = message;
            Image = image;
            Date = date;
        }

        public User User { get; set; }
        public Admin Admin { get; set; }
        public Lawyer Lawyer { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public DateTime Date { get; set; }
    }
}
