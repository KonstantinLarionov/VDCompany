using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompanyMVC.Models.Objects;

namespace VDCompanyMVC.Models.Pages
{
    public class ModelAdminCases
    {
        public List<Case> Cases { get; set; }
    }
    public class ModelAdminLawyers
    { 
        public List<Lawyer> Lawyers { get; set; }
    }
    public class ModelAdminUsers
    { 
        public List<User> Users { get; set; }
    }
}
