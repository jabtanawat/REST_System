using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class SF_Payment
    {
        [Key]
        public string PaymentId { get; set; }
        public string BillId { get; set; }
        public string TableId { get; set; }
        public string MemberId { get; set; }
        public int St { get; set; }
        public DateTime Dates { get; set; }
        public decimal SumBalance { get; set; }
        public decimal Balance { get; set; }
        public decimal ServicePersen { get; set; }
        public decimal ServiceBath { get; set; }
        public decimal MemberPersen { get; set; }
        public decimal MemberBath { get; set; }
        public decimal Persen { get; set; }
        public decimal PersenBath { get; set; }
        public decimal VatPersen { get; set; }
        public decimal VatBath { get; set; }
        public decimal BeforeVat { get; set; }
        public decimal AfterVat { get; set; }
        public decimal Coupon { get; set; }
        public decimal Total { get; set; }
        public decimal MoneyPut { get; set; }
        public decimal MoneyChange { get; set; }
        public Boolean Cash1 { get; set; } // เงินสด
        public decimal Cash1Bath { get; set; }
        public Boolean Cash2 { get; set; } // เงินโอน
        public decimal Cash2Bath { get; set; }
        public Boolean Cash3 { get; set; } // บัตรเครดิต
        public decimal Cash3Bath { get; set; }
        public int Cash3Type { get; set; }
        public string Cash3Number { get; set; } // เลขบัตรเครดิต
        // -------------------------------------------------------
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
