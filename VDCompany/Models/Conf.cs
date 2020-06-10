﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompany.Models
{
    public static class Conf
    {
        public static string ConnectDb { get; private set; } = "Server=localhost;Database=VDCompany;User=vddbadmin;Password=4Thehorde!;";
    }
    public static class MessagesUser
    {
        public static string MessageCaseOk { get; private set; } = "Ваше дело передано на рассмотрение. Мы свяжемся с Вами в ближайшее время";
        public static string MessageCaseFail { get; private set; } = "Ваше дело не создано, возможны проблемы с авторизацией, обновите страницу!";
    }
}
