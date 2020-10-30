using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class SF_OrderSub
    {
        [Key]
        [Required]
        public string OrderId { get; set; }
        [Required]
        public int i { get; set; }
        [Required]
        public string TableId { get; set; }
        [Required]
        public string FoodId { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public string BranchId { get; set; }
    }
}
