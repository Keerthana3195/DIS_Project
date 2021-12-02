using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIS_Project.Models
{
    public class Drug
    {
        public string ProductDescription { get; set; }
        public string RecallInitDate { get; set; }
        public string Classification { get; set; }
        public string Reason { get; set; }
        public string Voluntary_Mandate { get; set; }

        public string Country { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Distribution { get; set; }
    }
}
