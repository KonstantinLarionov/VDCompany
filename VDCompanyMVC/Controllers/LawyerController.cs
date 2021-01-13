using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using VDCompanyMVC.Controllers.Core.LawyerCore;
using VDCompanyMVC.Models.DTO;
using VDCompanyMVC.Models.Entitys;
using VDCompanyMVC.Models.Objects;
using VDCompanyMVC.Models.Pages;

namespace VDCompany.Controllers
{
    public class LawyerController : Controller
    {
        private static readonly StartContext db = new StartContext(new DbContextOptions<StartContext>());
        private (string login, string password) userinfo = (null, null);
        private Lawyer curruser = null;
        #region AUTHLOGIN
        public bool Auth()
        {
            try
            {
                var login = HttpContext.Session.GetString("login");
                var password = HttpContext.Session.GetString("password");
                userinfo = (login, password);
                curruser = db.Lawyers.Where(f => f.Login == login && f.Password == password).FirstOrDefault();
                return db.Lawyers.Any(f => f.Login == login && f.Password == password);
            }
            catch
            {
                return false;
            }
        }
        #endregion
        public IActionResult Cases()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            var cas = db.LawyersCases.Where(f => f.LawyerId == curruser.Id).ToList();
            var allcases = new List<Case>();
            foreach (var item in cas)
            {
                var _case = db.Cases.Where(f => f.Id == item.CaseId).FirstOrDefault();
                if(_case != null)
                    allcases.Add(_case);
            }
            var model = new ModelLawyerCases
            {
                Cases = allcases
            };
            return View(model);
        }
        public IActionResult Case(int Id)
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            var _case = db.Cases.Where(f => f.Id == Id).Include(d => d.Docs).FirstOrDefault();
            var user = db.Users.Where(f => f.Login == userinfo.login && f.Password == userinfo.password).FirstOrDefault();
            var index_lawyers = db.LawyersCases.Where(f => f.CaseId == Id).ToList();
            var lawyers_in_case = new List<Lawyer>();
            foreach (var item in index_lawyers)
            {
                var l = db.Lawyers.Where(f => f.Id == item.LawyerId).FirstOrDefault();
                lawyers_in_case.Add(l);
            }
            MyCaseDTO myCaseDTO = new MyCaseDTO
                (
                    _case,
                    user
                );
            myCaseDTO.Lawyers = lawyers_in_case;
            return View(myCaseDTO);
        }
        public IActionResult Index()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });

            return RedirectToRoute(new { controller = "Lawyer", action = "Cases" });
        }
        public IActionResult PDN()
        {
            if (!Auth())
                return RedirectToRoute(new { controller = "User", action = "Login" });
            return View();
        }
    }
}
