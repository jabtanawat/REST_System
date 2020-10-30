using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class SF_Order
    {
        [Key]
        [Required]
        public string OrderId { get; set; }
        [Required]
        public string TableId { get; set; }
        public DateTime OrderDt { get; set; }
        public decimal PriceTotal { get; set; }
        public int ST { get; set; }
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
