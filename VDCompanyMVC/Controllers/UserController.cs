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

namespace VDCompany.Controllers
{
    public class UserController : Controller
    {
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
