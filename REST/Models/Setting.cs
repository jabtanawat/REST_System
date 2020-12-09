using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class Setting
    {
        [Key]
        public string id { get; set; }
        public decimal TaxSell { get; set; }
        public decimal TaxBuy { get; set; }
        public decimal Service { get; set; }
        public decimal Point { get; set; }
        public Boolean Perbunch { get; set; }
    }
}
