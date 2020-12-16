using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewOrder
    {
        public string OrderId { get; set; }
        public int i { get; set; }
        public string TableId { get; set; }
        public string FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Option { get; set; }
    }
}
