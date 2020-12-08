using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewTb_SF_Order
    {
        public string OrderId { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public DateTime OrderDt { get; set; }
        public string Dates { get; set; }
        public decimal PriceTotal { get; set; }
        public int ST { get; set; }
        public string Name { get; set; }
    }
}
