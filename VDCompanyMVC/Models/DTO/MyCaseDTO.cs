using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompanyMVC.Models.Objects;

namespace VDCompanyMVC.Models.DTO
{
    public class MyCaseDTO
    {
        public Case Case = new Case();
        public User User = new User();
        public Admin Admin = new Admin();
        public Lawyer Lawyer = new Lawyer();
        public List<Lawyer> Lawyers = new List<Lawyer>();

        public MyCaseDTO(Case @case, User user)
        {
            Case = @case;
            User = user;
        }
        public MyCaseDTO(Case @case, Admin user)
        {
            Case = @case;
            Admin = user;
        }
        public MyCaseDTO(Case @case, Lawyer user)
        {
            Case = @case;
            Lawyer = user;
        }
    }
}
