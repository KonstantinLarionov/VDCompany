
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using VDCompanyMVC.Models.Entitys;
using VDCompanyMVC.Models.Objects.chat;

namespace VDCompanyMVC.Hubs
{
    public class Telegram
    {
        private string Token { get; set; }
        private string Api { get; set; } = "https://api.telegram.org/bot";
        private string MethodeSendMessage { get; set; } = "/sendMessage";

        public Telegram(string token)
        {
            Token = token;
        }

        public void SendMessage(string text, string chatId)
        {
            using (var webClient = new WebClient())
            {
                var response = webClient.DownloadString("https://afcstudio.ru/core/telegram.php?token=" + Token + "&text=" + text + "&chatid=" + chatId);
            }
        }
    }
    public class ChatHub : Hub
    {
        private static ChatContext chat = new ChatContext((new DbContextOptions<ChatContext>()));

        private string mytoken = "69ae97f121aa0daf0a0c225a73c16e5782fa448";

        public string Connect(string login)
        {
            var user = chat.Users.Where(x => x.Login == login).FirstOrDefault();
            if (user != null)
            {
                var dialog = chat.Dialogs.Where(x => x.Identity == user.UserIdentity).FirstOrDefault();
                dialog.Identity = Context.ConnectionId;
                user.UserIdentity = Context.ConnectionId;
                user.Date = DateTime.Now;
                user.IP = Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
                chat.SaveChanges();
                return "{\"status\":\"updated\"}";
            }
            else 
            {
                user = new User()
                {
                    UserIdentity = Context.ConnectionId,
                    Login = login,
                    Date = DateTime.Now,
                    IP = Context.GetHttpContext().Connection.RemoteIpAddress.ToString()
                };
                chat.Users.Add(user);
                chat.Dialogs.Add(new Dialog()
                    { 
                        Identity = Context.ConnectionId,
                        Users = new List<User>() { user } ,
                        DateCreate = DateTime.Now
                    });
                chat.SaveChanges();
                return "{\"status\":\"created\"}";
            }
        }


        public async Task Send1(string message, string login="-", string password="-", string dialogId="")
        {
            if (password == null || password == "-" || password == "")
            {
                User user = null;
                if (login != "-" || login != null || login != "")
                {
                    if (chat.Users.Any(x => x.Login == login))
                    {
                        user = chat.Users.Where(x => x.Login == login).FirstOrDefault();
                        user.UserIdentity = Context.ConnectionId;
                        chat.SaveChanges();
                    }
                    else
                    {
                        user = new User()
                        {
                            Date = DateTime.Now,
                            Login = login,
                            UserIdentity = Context.ConnectionId,
                            IP = this.Context.GetHttpContext().Connection.RemoteIpAddress.ToString()
                        };
                        chat.Users.Add(user);
                        chat.SaveChanges();
                    }
                }
                else 
                {
                    if (chat.Users.Any(x => x.UserIdentity == Context.ConnectionId))
                    {
                        user = chat.Users.Where(x => x.UserIdentity == Context.ConnectionId).FirstOrDefault();
                    }
                    else
                    {
                        user = new User()
                        {
                            Date = DateTime.Now,
                            UserIdentity = Context.ConnectionId,
                            IP = this.Context.GetHttpContext().Connection.RemoteIpAddress.ToString()
                        };
                        chat.Users.Add(user);
                        chat.SaveChanges();
                    }
                }
                Message messageNew = new Message() { Date = DateTime.Now, Text = message, User = user };

                Dialog dialog = null;
                if (chat.Dialogs.Any(x => x.Identity == user.UserIdentity))
                {
                    dialog = chat.Dialogs.Where(x => x.Identity == user.UserIdentity).Include(x => x.Users).Include(x => x.Messages).FirstOrDefault();
                    dialog.Messages.Add(messageNew);
                    chat.SaveChanges();
                }
                else
                {
                    dialog = new Dialog() { Identity = user.UserIdentity, DateCreate = DateTime.Now, Users = new List<User>() { user }, Messages = new List<Message>() { messageNew } };
                    chat.Dialogs.Add(dialog);
                    chat.SaveChanges();
                }

                foreach (var userD in dialog.Users)
                {
                    await Clients.Client(userD.UserIdentity).SendAsync("Send", JsonSerializer.Serialize(Tuple.Create(user, message)));
                }
            }
            else
            {
                User admin = null;
                if (chat.Users.Any(x => x.Login == login && x.Password == password && x.Role == Role.Admin))
                {
                    admin = chat.Users.Where(x => x.Login == login && x.Password == password).FirstOrDefault();
                    admin.UserIdentity = Context.ConnectionId;
                }
                else { return; }

                Message messageNew = new Message() { Date = DateTime.Now, Text = message, User = admin };

                var dialog = chat.Dialogs.Where(x => x.Identity == dialogId).Include(x => x.Users).Include(x => x.Messages).FirstOrDefault();
                if (!dialog.Users.Any(x=>x.Login == admin.Login && x.Password == admin.Password))
                {
                    dialog.Users.Add(admin);
                } 
                dialog.Messages.Add(messageNew);
                chat.SaveChanges();
                foreach (var user in dialog.Users)
                {
                    await Clients.Client(user.UserIdentity).SendAsync("Send", JsonSerializer.Serialize(Tuple.Create(admin, message)));
                }
            }
        }
        
        public async Task Send(string message, string login = "-", string password = "-", string to = "", string token = "-")
        {
            message = message.Replace("<", "[").Replace(">", "]");
            User user = null;
            Dialog dialog = null;

            if (password == "-" || password == "" || password == null) // Клиент
            {
                user = chat.Users.Where(x => x.UserIdentity == Context.ConnectionId).FirstOrDefault();
                if (user != null) // Если такой юзер уже есть 
                {
                    dialog = chat.Dialogs.Where(x => x.Identity == user.UserIdentity).Include(x => x.Users).Include(x => x.Messages).FirstOrDefault();
                    dialog.Messages.Add(
                    new Message()
                    {
                        Text = message,
                        User = user,
                        Date = DateTime.Now
                    });
                    chat.SaveChanges();
                }
                else // Иначе создаем нового юзера
                {
                    int k = 1;
                    string newlogin = "";
                    while (k > 0)
                    {
                        newlogin = GenNewPsw(6);
                        k = chat.Users.Where(x => x.Login == newlogin).Count();
                    }
                    user = new User()
                    {
                        Date = DateTime.Now,
                        UserIdentity = Context.ConnectionId,
                        Login = newlogin,
                        IP = Context.GetHttpContext().Connection.RemoteIpAddress.ToString()
                    };
                    dialog = new Dialog()
                    {
                        Identity = Context.ConnectionId,
                        DateCreate = DateTime.Now,
                        Users = new List<User>() { user },
                        Messages = new List<Message>()
                            {
                                new Message()
                                {
                                    Text = message,
                                    User = user,
                                    Date = DateTime.Now
                                }
                            }
                    };
                    chat.Users.Add(user);
                    chat.Dialogs.Add(dialog);
                    chat.SaveChanges();
                }

                foreach (var userD in dialog.Users)
                {
                    await Clients.Client(userD.UserIdentity).SendAsync("Send", JsonSerializer.Serialize(Tuple.Create(user.Login, message)));
                }
                /*var s = VK.Send("random_id=" + DateTime.Now.ToString("yyMMddHHmmss") +
                          "&v=" + "5.92" +
                          "&user_ids=" + "198672105, 184563259, 153632713" + //, 184563259, 153632713
                          "&message=" + "Новое сообщение в чате от " + user.Login +
                          "&access_token=" + "a9f58fdb7384b1c507b0aa0244b20f11ae8e0315bc85cd25a73c16e5782fa44869ae97f121aa0daf0a0c2");*/
            }
            else
            {
                if (token != mytoken)
                    return;
                if (chat.Users.Any(x => x.Login == login && x.Password == password && x.Role == Role.Admin))
                {
                    user = chat.Users.Where(x => x.Login == login && x.Password == password).FirstOrDefault();
                    user.UserIdentity = Context.ConnectionId;
                }
                else { return; }

                Message messageNew = new Message() { Date = DateTime.Now, Text = message, User = user };
                var u = chat.Users.Where(f => f.Login == to).FirstOrDefault();
                dialog = chat.Dialogs.Where(x => x.Identity == u.UserIdentity).Include(x => x.Users).Include(x => x.Messages).FirstOrDefault();
                if (!dialog.Users.Any(x => x.Login == user.Login && x.Password == user.Password))
                {
                    dialog.Users.Add(user);
                }
                dialog.Messages.Add(messageNew);

                chat.SaveChanges();

                foreach (var us in dialog.Users)
                {
                    await Clients.Client(us.UserIdentity).SendAsync("Send", JsonSerializer.Serialize(Tuple.Create(user.Login, message)));
                }
            }
        }
        private string GetHash(string data, int length = 0)
        {
            var tmpSource = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
            byte[] tmpNewHash = new System.Security.Cryptography.SHA256CryptoServiceProvider().ComputeHash(tmpSource);
            string h = "";
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
        /// <summary>
        /// AdminHandleMessage
        /// </summary>
        /// <param name="dialogId">Where recieve message</param>
        /// <param name="message">message</param>
        /// <param name="login">Login admin</param>
        /// <param name="password">Password admin</param>
        /// <returns></returns>
        public async Task SendAdmin(string dialogId, string message, string login, string password)
        {
            
        }

    }
    public static class VK
    {
        public static string Send(string data)
        {
            using (WebClient w = new WebClient())
            {
                w.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string result = w.UploadString("https://api.vk.com/method/messages.send", data);
            }
            return "1";
        }
    }
}
