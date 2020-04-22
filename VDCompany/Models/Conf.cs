using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompany.Models
{
    public static class Conf
    {
        public static string ConnectDb { get; private set; } = "Server=localhost;Database=VDCompany;User=vddbadmin;Password=4Thehorde!;";
    }
}
