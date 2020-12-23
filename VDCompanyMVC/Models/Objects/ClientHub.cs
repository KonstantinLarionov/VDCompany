using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public class ClientHub
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; }
        public DateTime LastNotify { get; set; }
    }
}
