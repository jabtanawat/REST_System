using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewST_TranSub
    {
        public string Documents { get; set; }
        public string StapleId { get; set; }
        public string StapleName { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string Vat { get; set; }
    }
}
