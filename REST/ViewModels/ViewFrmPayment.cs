using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewFrmPayment
    {
        public string TableId { get; set; }
        public string TableName { get; set; }
        public int St { get; set; }
        public string Status { get; set; }
        public string Total { get; set; }
        public decimal Persen { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string Balance { get; set; }
        public decimal MemberPersen { get; set; }
        public decimal Rebate { get; set; }
        public decimal Score { get; set; }
        public decimal MoneyPut { get; set; }
        public decimal MoneyChange { get; set; }
        public List<ViewSF_OrderSub> OrderSub { get; set; }
    }
}
