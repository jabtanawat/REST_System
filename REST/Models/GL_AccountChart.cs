using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class GL_AccountChart
    {
        [Key]
        public int Type { get; set; }
        public string AccNo { get; set; }
        public string AccName { get; set; }
        public int DrCr { get; set; }
        // -------------------------------------------------------
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
