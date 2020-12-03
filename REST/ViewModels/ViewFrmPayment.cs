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
        public string Balance { get; set; }
        public string ServicePersen { get; set; }
        public string ServiceBath { get; set; }
        public string MemberPersen { get; set; }
        public string MemberBath { get; set; }
        public string Persen { get; set; }
        public string PersenBath { get; set; }
        public string Sum { get; set; }
        public string Coupon { get; set; }
        public string Total { get; set; }
        public string SumBalance { get; set; }
        public Boolean cash1 { get; set; } // เงินสด checkbox
        public string BathCash1 { get; set; } // เงินสด
        public Boolean cash2 { get; set; } // เงินโอน checkbox
        public string BathCash2 { get; set; } // เงินโอน
        public Boolean cash3 { get; set; } // บัตรเครดิต checkbox
        public string BathCash3 { get; set; } // บัตรเครดิต
        public string NumberCash3 { get; set; } // เลขบัตรเครดิต
        public decimal MoneyPut { get; set; }
        public decimal MoneyChange { get; set; }


        public List<ViewSF_BillSub> BillSub { get; set; }
    }
}
