using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIS_Project.Models
{
    public class AllData
    {
        public List<Food> FoodList { get; set; }
        public List<Drug> DrugList { get; set; }
        public Food Foodobj { get; set; }
        public Drug Drugobj { get; set; }
        public List<string> Countries { get; set; }
        public List<string> City { get; set; }
        public List<string> RecallInitDate { get; set; }
        public string SelectedCity { get; set; }
        
    }
}
