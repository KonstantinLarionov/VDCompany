using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Objects;

namespace VDCompany.Models.DTO
{
    public class AdminLawyersDTO
    {
        public AdminLawyersDTO(List<Lawyer> lawyers, List<Case> cases)
        {
            Lawyers = lawyers;
            Cases = cases;
        }

        public List<Lawyer> Lawyers { get; set; } = new List<Lawyer>();
        public List<Case> Cases { get; set; } = new List<Case>();
    
    }
}
