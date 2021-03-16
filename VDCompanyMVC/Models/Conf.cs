using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models
{
    public static class Conf
    {
#if DEBUG
        public static string ConnectDb = "Server=localhost;Database=u0967433_vd;User=u0967_vd;Password=$lI2bg26;";
        public static string DBChat { get; private set; } = "Server=localhost;Database=u0967433_vd_chat;User=u0967_vd;Password=$lI2bg26;";
#else
        public static string ConnectDb = "Server=localhost;Database=u0967433_vd;User=u0967_vd;Password=$lI2bg26;";
        public static string DBChat { get; private set; } = "Server=localhost;Database=u0967433_vd_chat;User=u0967_vd;Password=$lI2bg26;";
#endif
    }
}
