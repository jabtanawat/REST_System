using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class ST_TranManual
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int i { get; set; }
        public string StapleId { get; set; }
        public int Trans { get; set; }
        public DateTime Dates { get; set; }
        public string Times { get; set; }
        public decimal After { get; set; }
        public decimal Edit { get; set; }
        public decimal Balance { get; set; }
        public string BranchId { get; set; }
    }
}
