using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompany.Models.Objects
{
    public enum TypeCase 
    {
        Close, Open, InProcess
    }
    public class Case
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public List<Doc> Docs { get; set; }
        public List<Lawyer> Lawyers { get; set; }
        public Dialog Dialog { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public TypeCase Status { get; set; }
    }
}
