using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VDCompany.Controllers.Core.LawyerCore;
using VDCompany.Models.DTO;
using VDCompany.Models.Objects;

namespace VDCompany.Controllers
{

    public class LawyersController : Controller
    {
        public IActionResult Index()
        {
            return View(HttpContext.SendToLaw(x => x.GetIndexDTO()));
        }
    }
}
