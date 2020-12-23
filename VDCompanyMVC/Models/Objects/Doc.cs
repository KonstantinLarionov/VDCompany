using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public enum TypeDoc
    {
        NONE, IMG, XLC, WORD, PDF, AUDIO, VIDEO
    }
    public class Doc
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public DateTime DateAdd { get; set; }
        public TypeDoc Type { get; set; }
    }
}
