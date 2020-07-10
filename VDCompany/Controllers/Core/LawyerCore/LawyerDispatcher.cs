using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Objects;

namespace VDCompany.Controllers.Core.LawyerCore
{
    public static partial class Extensions
    {
        #region PrivateInstance
        private static LawyerDispatcher Instance(HttpContext httpcon)
        {
            var u = LawyerDispatcher.GetInstance();
            u.HttpContext = httpcon;
            return u;
        }
        #endregion

        #region PublicInterfaceForUser
        public static List<User> SendToLaw(this HttpContext httpcon, Func<LawyerDispatcher, List<User>> func)
        {
            return func(Instance(httpcon));
        }
        public static List<Case> SendToLaw(this HttpContext httpcon, Func<LawyerDispatcher, List<Case>> func)
        {
            return func(Instance(httpcon));
        }
        public static Case SendToLaw(this HttpContext httpcon, Func<LawyerDispatcher, Case> func)
        {
            return func(Instance(httpcon));
        }
        public static Lawyer SendToLaw(this HttpContext httpcon, Func<LawyerDispatcher, Lawyer> func)
        {
            return func(Instance(httpcon));
        }
        #endregion
    }
    public class LawyerDispatcher
    {
        #region PrivateProps
        private HttpContext http = null;
        private static LawyerDigger digger = null;
        private static LawyerDispatcher user_dispatcher = null;
        private delegate void CreateDigger(HttpContext http);
        private static event CreateDigger digger_creator;
        #endregion
        #region PublicProps
        public LawyerDispatcher Dispatcher => user_dispatcher;
        private static void EventCreateDigger(HttpContext http) => digger = new LawyerDigger(http);
        public HttpContext HttpContext
        {
            get => http;
            set
            {
                http = value;
                digger_creator?.Invoke(value);
            }
        }
        private LawyerDispatcher() : base() { }
        public static LawyerDispatcher GetInstance()
        {
            if (user_dispatcher == null)
            {
                user_dispatcher = new LawyerDispatcher();
                digger_creator += EventCreateDigger;
            }
            return user_dispatcher;
        }
        #endregion

    }
}
