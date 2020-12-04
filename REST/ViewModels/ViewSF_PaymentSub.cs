using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewSF_PaymentSub
    {
        public string PaymentId { get; set; }
        public int i { get; set; }
        public string FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public string BranchId { get; set; }
    }
}
