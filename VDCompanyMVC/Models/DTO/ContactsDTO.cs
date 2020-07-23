using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Objects;

namespace VDCompany.Models.DTO
{
    public class ContactsDTO
    {
        public ContactsDTO(List<Lawyer> lawyers, ServiceVDContacts serviceVD)
        {
            Lawyers = lawyers;
            ServiceVD = serviceVD;
        }

        public List<Lawyer> Lawyers { get; set; } = new List<Lawyer>();
        public ServiceVDContacts ServiceVD { get; set; } = new ServiceVDContacts();
    }
}
