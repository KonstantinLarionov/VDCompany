using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompanyMVC.Models.Objects;

namespace VDCompanyMVC.Models.DTO
{
    public class IndexDTO
    {
        public IndexDTO(List<User> users, List<Case> cases, List<Lawyer> lawyers, int countUsers, int persentUpCases, int countCases, int goodFeedBack)
        {
            Users = users;
            Cases = cases;
            Lawyers = lawyers;
            CountUsers = countUsers;
            PersentUpCases = persentUpCases;
            CountCases = countCases;
            GoodFeedBack = goodFeedBack;
        }

        public List<User> Users { get; set; }
        public List<Case> Cases { get; set; }
        public List<Lawyer> Lawyers { get; set; }
        public int CountUsers { get; set; }
        public int PersentUpCases { get; set; }
        public int CountCases { get; set; }
        public int GoodFeedBack { get; set; }
    }
}
