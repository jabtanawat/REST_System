using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewSF_Bill
    {
        public string BillId { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public DateTime BillDt { get; set; }
        public string Dates { get; set; }
        public decimal PriceTotal { get; set; }
        public string SumBalance { get; set; }
    }

}
