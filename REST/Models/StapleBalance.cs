using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class StapleBalance
    {
        [Key]
        public string StapleId { get; set; }
        public decimal QtyBalance { get; set; }
        public string BranchId { get; set; }
    }
}
