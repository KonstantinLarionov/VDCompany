using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VDCompany.Models.Objects;
using VDCompany.Models.DTO;
using VDCompany.Models.Entitys;
using Microsoft.EntityFrameworkCore;
using VDCompany.Models.Secur;
using System.Data.Entity.Validation;
using Microsoft.AspNetCore.Http;
using VDCompany.Testings;

namespace VDCompany.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UserCore.UserDispatcher user;
        public UserController()
        {
            user = new UserCore.UserDispatcher(HttpContext);
        }
       
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
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
        [HttpPost(Name = "Create")]
        [Route("Create")]
        public string Create([FromBody] Case newcase)
        {

            Case create = new Case();
            create.Name = newcase.Name;
            create.Type = newcase.Type;
            create.Description = newcase.Description;
            create.DateStart = DateTime.Now;
            db.Cases.Add(create);
            db.SaveChanges();
            return "Ваше дело передано на рассмотрение. Мы свяжемся с Вами в ближайшее время";
        }
        [HttpPost(Name = "GetBills")]
        [Route("GetBills")]
        public List<Bill> GetBills()
        {
            /*if (Auth())
            {*/
            UserDBBuilder.HttpContext = HttpContext;
            UserDBBuilder.Build("cookies");
            
            var userWithBills = GetUser().Include(x => x.Bills).FirstOrDefault();
            return userWithBills.Bills;
            /*}
            else 
            {
                return null;
            }*/
        }

        [HttpPost(Name = "ChangeStatus")]
        [Route("ChangeStatus")]
        public object[] ChangeStatus([FromBody] int id )
        {
            user.GetUser().Include(x => x.Bills).FirstOrDefault();
            user.Bills.Where(x => x.Id == id).FirstOrDefault().Status = StatusBill.InProcess;
            return new object[] { true, id };
        }

       

    }
}
