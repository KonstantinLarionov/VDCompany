using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.DTO;
using VDCompany.Models.Objects;

namespace VDCompany.Controllers.Core.AdminCore
{
    public static partial class Extensions
    {
        #region PrivateInstance
        private static AdminDispatcher Instance(HttpContext httpcon)
        {
            var u = AdminDispatcher.GetInstance();
            u.HttpContext = httpcon;
            return u;
        }
        #endregion

        #region PublicInterfaceForAdmin
        public static List<User> SendToAdmin(this HttpContext httpcon, Func<AdminDispatcher, List<User>> func)
        {
            return func(Instance(httpcon));
        }
        public static User SendToAdmin(this HttpContext httpcon, Func<AdminDispatcher, User> func)
        {
            return func(Instance(httpcon));
        }
        public static List<Lawyer> SendToAdmin(this HttpContext httpcon, Func<AdminDispatcher, List<Lawyer>> func)
        {
            return func(Instance(httpcon));
        }
        public static Lawyer SendToAdmin(this HttpContext httpcon, Func<AdminDispatcher, Lawyer> func)
        {
            return func(Instance(httpcon));
        }
        public static List<Case> SendToAdmin(this HttpContext httpcon, Func<AdminDispatcher, List<Case>> func)
        {
            return func(Instance(httpcon));
        }
        public static Case SendToAdmin(this HttpContext httpcon, Func<AdminDispatcher, Case> func)
        {
            return func(Instance(httpcon));
        }
        public static IndexDTO SendToAdmin(this HttpContext httpcon, Func<AdminDispatcher, IndexDTO> func)
        {
            return func(Instance(httpcon));
        }
        public static void SendToAdmin(this HttpContext httpcon, Action<AdminDispatcher> func)
        {
            func(Instance(httpcon));
        }
        #endregion
    }
    public class AdminDispatcher
    {
        #region PrivateProps
        private HttpContext http = null;
        private static AdminDigger digger = null;
        private static AdminDispatcher admin_dispatcher = null;
        private delegate void CreateDigger(HttpContext http);
        private static event CreateDigger digger_creator;
        #endregion
        #region PublicProps
        public AdminDispatcher Dispatcher => admin_dispatcher;
        private static void EventCreateDigger(HttpContext http) => digger = new AdminDigger(http);
        public HttpContext HttpContext
        {
            get => http;
            set
            {
                http = value;
                digger_creator?.Invoke(value);
            }
        }
        private AdminDispatcher() : base() { }
        public static AdminDispatcher GetInstance()
        {
            if (admin_dispatcher == null)
            {
                admin_dispatcher = new AdminDispatcher();
                digger_creator += EventCreateDigger;
            }
            return admin_dispatcher;
        }
        #endregion
        #region InfoTakers
        public IndexDTO GetIndexDTO()
        {
            int cu = digger.GetUsersCount();
            int cugf = cu > 4 ? cu - 3 : cu;
            IndexDTO indexDTO = new IndexDTO
                (
                    digger?.GetUsers(20),
                    digger?.GetCases(20),
                    digger?.GetLawyers(),
                    cu,
                    92,
                    digger.GetCasesCount(),
                    cugf
                );
            return indexDTO;
        }
        public void SetBillToUser(int idUser, string nameCase, DateTime datePut, DateTime dateEnd, string whoPut, string whoTake, string sum, string dopSum, string itogo, string requisit)
        {
            digger?.SetBill(idUser, new Bill()
            {
                Amount = Convert.ToDouble(itogo.Replace(".", ",")),
                DateCreate = datePut,
                DatePay = dateEnd,
                WhoPut = whoPut,
                NameCase = nameCase,
                Status = StatusBill.InProcess,
                Requizit = requisit
            });
        }

        public void SetLaws(int idcase, int law1,int law2, int law3, int law4, int law5)
        {
            var law_form_db1 = digger?.GetLawyer(law1);
            digger?.SetLawyer(idcase, law_form_db1);
            var law_form_db2 = digger?.GetLawyer(law2);
            digger?.SetLawyer(idcase, law_form_db2);
            var law_form_db3 = digger?.GetLawyer(law3);
            digger?.SetLawyer(idcase, law_form_db3);
            var law_form_db4 = digger?.GetLawyer(law4);
            digger?.SetLawyer(idcase, law_form_db4);
            var law_form_db5 = digger?.GetLawyer(law5);
            digger?.SetLawyer(idcase, law_form_db5);
        }
        #endregion

    }
}
