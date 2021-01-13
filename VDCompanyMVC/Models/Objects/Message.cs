using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public class Message
    {
        public int Id { get; set; }
        public int DialogId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public Admin Admin { get; set; }
        public Lawyer Lawyer { get; set; }
    }
}
