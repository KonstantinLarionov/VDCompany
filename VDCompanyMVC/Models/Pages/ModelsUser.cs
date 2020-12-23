using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompanyMVC.Models.Objects;

namespace VDCompanyMVC.Models.Pages
{
    public class ModelUserCases
    {
        public List<Case> Cases { get; set; }
    }
    public class ModelUserCase
    {
        public Case Cases { get; set; }
    }
    public class ModelUserBills
    {
        public List<Bill> Bills { get; set; }
    }
}
