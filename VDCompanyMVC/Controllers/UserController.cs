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

    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController()
        {}

        [HttpPost(Name = "GetBills")]
        [Route("GetBills")]
        public List<Models.Objects.Bill> GetBills()
        {
            return HttpContext.SendToUser(u=>u.GetBills());
        }

        [HttpPost(Name = "CreateCase")]
        [Route("CreateCase")]
        public string Create([FromBody] CaseDTO newcase)
        {
            return HttpContext.SendToUser(u=> u.CreateCase(newcase)) ? MessagesUser.MessageCaseOk : MessagesUser.MessageCaseFail ;
        }

        [HttpPost(Name = "ChangeStatus")]
        [Route("ChangeStatus")]
        public object[] ChangeStatus([FromBody] ChangerBillDTO CBD )
        {
            return new object[] { HttpContext.SendToUser(u => u.ChangeStateBill(CBD.Id)), CBD.Id };
        }

        [HttpPost(Name = "GetCases")]
        [Route("GetCases")]
        public List<Models.Objects.Case> GetCases()
        {
            return HttpContext.SendToUser(u => u.GetCases());
        }

        [HttpPost(Name = "GetLawyers")]
        [Route("GetLawyers")]
        public List<Models.Objects.Lawyer> GetLawyers()
        {
            return HttpContext.SendToUser(u => u.GetLawyers());
        }

    }
}
