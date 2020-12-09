using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class ST_TranSub
    {
        public string Documents { get; set; }
        public string StapleId { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int i { get; set; }
        public string Vat { get; set; }
        public string BranchId { get; set; }
    }
}
