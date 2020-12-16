using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class SF_BillSub
    {
        [Key]
        public string BillId { get; set; }
        public int i { get; set; }
        public string FoodId { get; set; }
        public int Status { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public string BranchId { get; set; }
    }
}
