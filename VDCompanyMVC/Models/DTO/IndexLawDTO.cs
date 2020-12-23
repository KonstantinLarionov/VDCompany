using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompanyMVC.Models.Objects;

namespace VDCompanyMVC.Models.DTO
{
    public class IndexLawDTO
    {
        public int CountUsers { get; set; }
        public int PersentUpCases { get; set; }
        public int CountCases { get; set; }
        public int GoodFeedBack { get; set; }

        public List<Case> Cases = new List<Case>();

        public IndexLawDTO(int countUsers, int persentUpCases, int countCases, int goodFeedBack, List<Case> cases)
        {
            CountUsers = countUsers;
            PersentUpCases = persentUpCases;
            CountCases = countCases;
            GoodFeedBack = goodFeedBack;
            Cases = cases;
        }
    }
}
