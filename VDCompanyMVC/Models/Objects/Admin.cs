using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public class Admin
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<ImageAdmin> Image { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string FIO { get; set; }
        public string About { get; set; }
        public string HASHPRVT { get; set; }
    }
}
