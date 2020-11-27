using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class ST_Trans
    {
        [Key]
        public string Documents { get; set; }
        public DateTime DateDocument { get; set; }
        public DateTime DateTax { get; set; }
        public string TaxNumber { get; set; }
        public string SupplierId { get; set; }
        public string Reference { get; set; }
        public decimal SumBalance { get; set; }
        // -------------------------------------------------------
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
