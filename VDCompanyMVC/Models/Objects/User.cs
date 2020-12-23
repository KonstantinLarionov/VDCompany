using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string RefCode { get; set; }
        public string FromeRef { get; set; }
        public string Email { get; set; }
        public string LASTIP { get; set; }
        public DateTime LASTLOGIN { get; set; }
        public DateTime DateReg { get; set; }
        public string Password { get; set; }
        public List<Case> Cases { get; set; } = new List<Case>();
        public List<Bill> Bills { get; set; } = new List<Bill>();
    }
}
