using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewFoodSub
    {
        public string FoodId { get; set; }
        public string StapleId { get; set; }
        public string StapleName { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }
    }
}
