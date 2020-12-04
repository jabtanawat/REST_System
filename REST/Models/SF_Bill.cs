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
        public int St { get; set; }
        public DateTime BillDt { get; set; }
        public decimal PriceTotal { get; set; }
        // -------------------------------------------------------
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
