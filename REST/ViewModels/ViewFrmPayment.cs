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
        public string MemberType { get; set; }
        public string Balance { get; set; }        
        public string ServicePersen { get; set; }
        public string ServiceBath { get; set; }
        public string MemberPersen { get; set; }
        public string MemberBath { get; set; }
        public string Persen { get; set; }
        public string PersenBath { get; set; }
        public string VatPersen { get; set; }
        public string VatBath { get; set; }
        public string BeforeVat { get; set; }
        public string AfterVat { get; set; }
        public string Sum { get; set; }
        public string Coupon { get; set; }
        public string Total { get; set; }
        public string SumBalance { get; set; }
        public Boolean Cash1 { get; set; } // เงินสด checkbox
        public string Cash1Bath { get; set; } // เงินสด
        public Boolean Cash2 { get; set; } // เงินโอน checkbox
        public string Cash2Bath { get; set; } // เงินโอน
        public Boolean Cash3 { get; set; } // บัตรเครดิต checkbox
        public string Cash3Bath { get; set; } // บัตรเครดิต
        public string Cash3Number { get; set; } // เลขบัตรเครดิต
        public string MoneyPut { get; set; }
        public string MoneyChange { get; set; }


        public List<ViewSF_BillSub> BillSub { get; set; }

        public List<ViewSF_OrderSub> OrderSub { get; set; }
    }
}
