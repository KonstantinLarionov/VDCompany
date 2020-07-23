using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompany.Models.Objects
{
    public class ServiceVDContacts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Familio { get; set; }
        public string Otch { get; set; }
        public string About { get; set; }
        public string PhoneAdmin { get; set; }
        public string PhoneSupport { get; set; }
        public string EmailAdmin { get; set; }
        public string EmailSupport { get; set; }
        public string Address { get; set; }
        public string AddressSupport { get; set; }
        public string LinkVK { get; set; }
        public string LinkFace { get; set; }
        public string Link { get; set; }
    }
}
