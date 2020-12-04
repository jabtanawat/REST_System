using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewSF_Payment
    {
        public string PaymentId { get; set; }
        public string BillId { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public string MemberType { get; set; }
        public int St { get; set; }
        public string Dates { get; set; }
        public string Total { get; set; }
        public string ServicePersen { get; set; }
        public string ServiceBath { get; set; }
        public string MemberPersen { get; set; }
        public string MemberBath { get; set; }
        public string Persen { get; set; }
        public string PersenBath { get; set; }
        public string Balance { get; set; }
        public string Coupon { get; set; }
        public string SumBalance { get; set; }
        public string MoneyPut { get; set; }
        public string MoneyChange { get; set; }
        public Boolean Cash1 { get; set; } // เงินสด
        public string Cash1Bath { get; set; }
        public Boolean Cash2 { get; set; } // เงินโอน
        public string Cash2Bath { get; set; }
        public Boolean Cash3 { get; set; } // บัตรเครดิต
        public string Cash3Bath { get; set; }
        public string Cash3Number { get; set; } // เลขบัตรเครดิต

        public List<ViewSF_PaymentSub> PaymentSub { get; set; }
    }
}
