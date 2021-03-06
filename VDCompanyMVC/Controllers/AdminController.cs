﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VDCompany.Controllers.Core.AdminCore;
using VDCompany.Models.DTO;
using VDCompany.Models.Objects;

namespace VDCompany.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View(HttpContext.SendToAdmin(x => x.GetIndexDTO()));
        }
        public IActionResult Contacts()
        {
            return View(HttpContext.SendToAdmin(x => x.GetContacts()));
        }
        public IActionResult Lawyers()
        {
            AdminLawyersDTO dTO = new AdminLawyersDTO
                (
                    HttpContext.SendToAdmin(x => x.GetLawyers()),
                    HttpContext.SendToAdmin(x => x.GetCases())
                );
            return View(dTO);
        }

        #region POST
        [HttpPost]
        public IActionResult SetBill(int iduser, string nameCase, DateTime datePut, DateTime dateEnd, string whoPut, string whoTake, string sum, string dopSum, string itogo, string requisit)
        {
            HttpContext.SendToAdmin(x=>x.SetBillToUser( 
                iduser,  
                nameCase,  
                datePut,  
                dateEnd,  
                whoPut,  
                whoTake, 
                sum,  
                dopSum,  
                itogo, requisit)
            );
            return View("Index",HttpContext.SendToAdmin(x => x.GetIndexDTO()));
        }

        [HttpPost]
        public IActionResult SetLaw(int idcase, int law1, int law2, int law3, int law4, int law5)
        {
            HttpContext.SendToAdmin(x=>x.SetLaws(idcase,law1,law2,law3,law4,law5));
            return View("Index", HttpContext.SendToAdmin(x => x.GetIndexDTO()));
        }

        [HttpPost]
        public IActionResult SetContacts(ServiceVDContacts contacts)
        {
            HttpContext.SendToAdmin(x=>x.SetContacts(contacts));
            return View("Contacts", HttpContext.SendToAdmin(x=>x.GetContacts()));
        }
        #endregion
    }
}
