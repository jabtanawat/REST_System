using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewStaple
    {
        public string StapleId { get; set; }
        public string StapleName { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }
        public int Tax { get; set; }
        public string QtyBalance { get; set; }
    }
}
