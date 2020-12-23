using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public enum StatusBill 
    {
        NotPaid, Paid, InProcess
    }
    public class Bill
    {
        public int Id { get; set; }
        public string WhoPut { get; set; }
        public string NameCase { get; set; }
        public double Amount { get; set; }
        public string Requizit { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DatePay { get; set; }
        public StatusBill Status { get; set; }
    }
}
