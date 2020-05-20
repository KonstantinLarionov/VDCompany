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

namespace VDCompany.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static StartContext db = new StartContext(new DbContextOptions<StartContext>());
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
        public string Create([FromBody] Create newcase)
        {

            Create create = new Create();
            create.Name = newcase.Name;
            create.Type = newcase.Type;
            create.Dialog = newcase.Dialog;
            create.DateStart = DateTime.Now;
            db.Creates.Add(create);
            db.SaveChanges();
            return "Ваше дело передано на рассмотрение. Мы свяжемся с Вами в ближайшее время";
        }
    }
}
