using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Objects;

namespace VDCompany.Models.DTO
{
    public class MyCaseDTO
    {
        public Case Case = new Case();
        public User User = new User();

        public MyCaseDTO(Case @case, User user)
        {
            Case = @case;
            User = user;
        }
    }
}
