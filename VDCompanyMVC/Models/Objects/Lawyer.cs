using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public class Lawyer
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string FIO { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
        public string Speciality { get; set; }
        public string PersonalCode { get; set; }
        public List<Image> Image { get; set; }
        public DateTime DateCreate { get; set; }
        public List<User> Users { get; set; }
    }
}
