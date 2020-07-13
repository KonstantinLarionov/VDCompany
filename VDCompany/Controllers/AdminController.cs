using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VDCompany.Controllers.Core.AdminCore;
using VDCompany.Models.DTO;

namespace VDCompany.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [Route("GetIndexDTO")]
        [HttpGet(Name ="GetIndexDTO")]
        public IndexDTO GetIndexDTO()
        {
            return HttpContext.SendToAdmin(x=>x.GetIndexDTO());
        }
    }
}
