using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.DTO;
using VDCompany.Models.Objects;
using VDCompany.Models.Secur;

namespace VDCompany.Controllers.Core.UserCore
{
    public static partial class Extensions
    {
        #region PrivateInstance
        private static UserDispatcher Instance(HttpContext httpcon)
        {
            var u = UserDispatcher.GetInstance();
            u.HttpContext = httpcon;
            return u;
        }
        #endregion

        #region PublicInterfaceForUser
        public static List<Bill> SendToUser(this HttpContext httpcon, Func<UserDispatcher, List<Bill>> func)
        {
            return func(Instance(httpcon));
        }
        public static List<Case> SendToUser(this HttpContext httpcon, Func<UserDispatcher, List<Case>> func)
        {
            return func(Instance(httpcon));
        }
        public static List<Lawyer> SendToUser(this HttpContext httpcon, Func<UserDispatcher, List<Lawyer>> func)
        {
            return func(Instance(httpcon));
        }
        public static bool SendToUser(this HttpContext httpcon, Func<UserDispatcher, bool> func)
        {
            return func(Instance(httpcon));
        }
        #endregion
    }
    public class UserDispatcher
    {
        #region PrivateProps
        private HttpContext http = null;
        private static UserDigger digger = null;
        private static UserDispatcher user_dispatcher = null;
        private delegate void CreateDigger(HttpContext http);
        private static event CreateDigger digger_creator;
        #endregion
        #region PublicProps
        public UserDispatcher Dispatcher => user_dispatcher;
        private static void EventCreateDigger(HttpContext http) => digger = new UserDigger(http);
        public HttpContext HttpContext
        {
            get => http;
            set {
                http = value;
                digger_creator?.Invoke(value);
            }
        }
        private UserDispatcher() : base() { }
        public static UserDispatcher GetInstance()
        {
            if (user_dispatcher == null)
            {
                user_dispatcher = new UserDispatcher();
                digger_creator += EventCreateDigger;
            }
            return user_dispatcher;
        }
        #endregion

        #region PublicMethods
        public List<Bill> GetBills()
        {
            return digger?.GetBills();
        }
        public List<Case> GetCases()
        {
            return digger?.GetCases();
        }
        public List<Lawyer> GetLawyers()
        {
            return digger?.GetLawyers();
        }
        public bool ChangeStateBill(int id)
        {
            var result = digger.ChangeStatusBill(id);
            return result == ResultState.Ok ? true : false ;
        }
        public bool CreateCase(CaseDTO caseDTO)
        {
            var result = digger.CreateCase(caseDTO);
            return SendEmail(caseDTO, result);
        }

        #endregion

        private bool SendEmail(CaseDTO caseDTO, ResultState result)
        {
            if (result == ResultState.Ok)
            {
                Mailler.SendEmailAsync(HttpContext.Session.GetString("login"), "VDCOMPANY", "Создание нового дела", $"Вы создали новое дело на сервисе VDCOMPANY! <br><br> Наименование вашего дела: {caseDTO.Name} <br> Тип вашего дела: {caseDTO.Type} <br> Дата создания: {DateTime.Now} <br><br> После регистрации, дело появится в вашем личном кабинете в списке дел и вам будет назначен подходящий специалист.<br><br> <span style=\"color:red;\">По всем вопросам: companyvd@yandex.ru</span>");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
