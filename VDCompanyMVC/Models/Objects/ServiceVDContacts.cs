using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public class ServiceVDContacts
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Familio { get; set; } = string.Empty;
        public string Otch { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string PhoneAdmin { get; set; } = string.Empty;
        public string PhoneSupport { get; set; } = string.Empty;
        public string EmailAdmin { get; set; } = string.Empty;
        public string EmailSupport { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string AddressSupport { get; set; } = string.Empty;
        public string LinkVK { get; set; } = string.Empty;
        public string LinkFace { get; set; } = string.Empty;
        public string LinkOK { get; set; } = string.Empty;
        public string Site { get; set; } = string.Empty;
    }
}
