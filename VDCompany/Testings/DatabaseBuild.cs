using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Entitys;
using VDCompany.Models.Interfaces;
using VDCompany.Models.Objects;

namespace VDCompany.Testings
{
    public enum ResultState 
    {
        OK, FAIL, CONTINUE
    }
    public static class UserDBBuilder
    {
        private const string TESTEMAIL = "kostya12277@yandex.ru";
        private const string TESTEPASSWORD = "123";
        private static StartContext entity;
        private static HttpContext http_context;
        public static StartContext Entity
        {
            private get { return null; }
            set { entity = value; }
        }
        public static HttpContext HttpContext
        {
            private get { return null; }
            set { http_context = value; }
        }


        public static void Build(params string[] arg)
        {
            if (arg.Contains("user"))
            {
                var result = arg.Contains("bill") ? CreateUserWithBill() : CreateUser();
            }
            if (arg.Contains("cookies"))
            {
                var result = CreateCookies();
            }
        }
        private static ResultState CreateUser()
        {
            try
            {
                var user = new User() { Email = TESTEMAIL, Password = TESTEPASSWORD, DateReg = DateTime.Now, LASTLOGIN = DateTime.Now };
                entity.Users.Add(user);
                entity.SaveChanges();
                return ResultState.OK;
            }
            catch (Exception ex)
            {
                return ResultState.FAIL;
            }
        }
        private static ResultState CreateUserWithBill()
        {
            try
            {
                var bill = new Bill() { Amount = 1000, DateCreate = DateTime.Now.Date, DatePay = DateTime.Now.Date.AddDays(1), NameCase = "TESTCASEBILL", Requizit = "4123 1231 2312 3121", Status = StatusBill.InProcess };
                var user = new User() { Email = TESTEMAIL, Password = TESTEPASSWORD, Bills = new List<Bill> { bill } };
                entity.Users.Add(user);
                entity.SaveChanges();
                return ResultState.OK;
            }
            catch (Exception ex)
            {
                return ResultState.FAIL;
            }
        }

        private static ResultState CreateCookies()
        {
            try 
            {
                http_context.Session.SetString("login", TESTEMAIL);
                http_context.Session.SetString("password", TESTEPASSWORD);
                return ResultState.OK;
            }
            catch (Exception ex)
            {
                return ResultState.FAIL;
            }
        }
    }
}
