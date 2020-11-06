using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewFrmPayment
    {
        public string Total { get; set; }
        public decimal Persen { get; set; }
        public string MemberId { get; set; }
        public string Balance { get; set; }
        public decimal MoneyPut { get; set; }
        public decimal MoneyChange { get; set; }
    }
}
