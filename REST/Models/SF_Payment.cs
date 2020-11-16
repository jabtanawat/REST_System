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
        public string TableId { get; set; }
        public decimal Total { get; set; }
        public decimal Persen { get; set; }
        public string MemberId { get; set; }
        public decimal Rebate { get; set; }
        public decimal Score { get; set; }
        public decimal Balance { get; set; }
        public int PayType { get; set; } // 1 = เงินสด, 2 = บัตรเครดิต
        public decimal MoneyPut { get; set; }
        public decimal MoneyChange { get; set; }
        public DateTime Dates { get; set; }
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
