using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewFrmPayment
    {
        public decimal Total { get; set; }
        public decimal Persen { get; set; }
        public string MemberId { get; set; }
        public decimal Balance { get; set; }
        public decimal MoneyPut { get; set; }
        public decimal MoneyChange { get; set; }
    }
}
