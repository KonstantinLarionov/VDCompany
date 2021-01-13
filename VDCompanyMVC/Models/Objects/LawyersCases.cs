using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDCompanyMVC.Models.Objects
{
    public class LawyersCases
    {
        public LawyersCases(int caseId, int lawyerId)
        {
            CaseId = caseId;
            LawyerId = lawyerId;
        }

        public int Id { get; set; }
        public int CaseId { get; set; }
        public int LawyerId { get; set; }
    }

}
