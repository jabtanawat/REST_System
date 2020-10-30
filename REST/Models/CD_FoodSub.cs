using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class CD_FoodSub
    {
        [Key]
        [Required]
        public string FoodId { get; set; }
        [Required]
        public string StapleId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string UnitId { get; set; }
        public string BranchId { get; set; }
    }
}
