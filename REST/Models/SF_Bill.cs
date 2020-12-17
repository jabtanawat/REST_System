using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class SF_Bill
    {
        [Key]
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
        // -------------------------------------------------------
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
