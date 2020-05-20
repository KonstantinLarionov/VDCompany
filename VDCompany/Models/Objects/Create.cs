using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompany.Models.Objects
{

    public class Create
    {
        public int Id { get; set; }
        public DateTime DateStart { get; set; } = DateTime.Now;
        public string Name { get; set; }
        public string Type { get; set; }
        public string Dialog { get; set; }
    }


}
