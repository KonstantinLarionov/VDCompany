using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models
{
    public static class Conf
    {
#if DEBUG
        public static string ConnectDb = "Server=localhost;Database=u0967433_vd;User=root;Password=root;";
        public static string DBChat { get; private set; } = "Server=localhost;Database=u0967433_vd_chat;User=root;Password=root;";
#else
        public static string ConnectDb = "Server=localhost;Database=u0967433_vd;User=u0967433_u0967_vd;Password=Rqzs34#2;";
        public static string DBChat { get; private set; } = "Server=localhost;Database=u0967433_vd_chat;User=u0967433_u0967_vd;Password=Rqzs34#2;";
#endif
    }
}
