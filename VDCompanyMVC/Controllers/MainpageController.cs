using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;


namespace VDCompany.Controllers
{
    [Route("server/[controller]")]
    [ApiController]
    public class MainpageController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }
        [HttpPost("{id}",Name = "Email")]
        public void PostEmail(string id, [FromForm]EmailReq emailReq)
        {
            EmailService emailService = new EmailService();
            emailService.SendEmailAsync("davidovadvokat@gmail.com", emailReq.Email, emailReq.Message).GetAwaiter().GetResult();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
    public class EmailReq {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Форма VDCompany", "azkon.supp@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Новое сообщение";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "Email: " + subject + "<br>Текст сообщения: <br><br>" + message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 587, false);
                await client.AuthenticateAsync("azkon.supp@yandex.ru", "4Ihxgae5dXreyY4rfLGN");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
