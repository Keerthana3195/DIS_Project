using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIS_Project.Models
{
    public class Food
    {
        public string Product { get; set; }
        public string Recall { get; set; }
        public string Classification { get; set; }
        public string Reason { get; set; }
        public string Mandate_Recall { get; set; }

        public string Country { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Distribution { get; set; }
    }
}
