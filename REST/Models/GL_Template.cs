using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class GL_Template
    {
        [Key]
        public string M_ID { get; set; }
        public string BookId { get; set; }
        public string M_Name { get; set; }
        public string M_DesCription { get; set; }
        public int M_PType { get; set; }
        // -------------------------------------------------------
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
