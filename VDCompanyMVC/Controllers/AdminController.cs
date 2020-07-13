using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VDCompany.Controllers.Core.AdminCore;
using VDCompany.Models.DTO;

namespace VDCompany.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View(HttpContext.SendToAdmin(x => x.GetIndexDTO()));
        }

    }
}
