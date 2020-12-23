using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VDCompanyMVC.Models.DTO;
using VDCompanyMVC.Models.Objects;

namespace VDCompanyMVC.Models.Pages
{
    public class ModelLawyerCases
    {
        public List<Case> Cases { get; set; }
    }
    public class ModelLawyerCase
    { 
        public Case Case { get; set; }
    }
}
