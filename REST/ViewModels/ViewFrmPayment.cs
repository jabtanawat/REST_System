using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewFrmPayment
    {
        public string BillId { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public string TableST { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string ServiceP { get; set; }
        public string ServiceB { get; set; }
        public string MemberP { get; set; }
        public string MemberB { get; set; }
        public string PersenP { get; set; }
        public string PersenB { get; set; }        
        public string Balance { get; set; }
        public string SumBalance { get; set; }
        public int PayType { get; set; } // 1 = เงินสด, 2 = บัตรเครดิต
        public decimal MoneyPut { get; set; }
        public decimal MoneyChange { get; set; }
        public List<ViewSF_BillSub> BillSub { get; set; }
    }
}
