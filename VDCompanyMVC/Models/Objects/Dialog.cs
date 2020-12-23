using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public class Dialog
    {
        public int Id { get; set; }
        public string Identity { get; set; }
        public List<User> Users { get; set; }
        public List<Lawyer> Lawyers { get; set; }
        public List<Admin> Admins { get; set; }
        public List<Message> Messages { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
