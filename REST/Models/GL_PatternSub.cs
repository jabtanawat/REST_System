using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class GL_PatternSub
    {
        [Key]
        public string P_ID { get; set; }
        public string AccNo { get; set; }
        public int DrCr { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }
        public int i { get; set; }
    }
}
