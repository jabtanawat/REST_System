using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class CD_Running
    {
        [Key]
        [Required]
        public string Name { get; set; }
        [Required]
        public string Front { get; set; }
        [Required]
        public string Number { get; set; }
        public Boolean AutoRun { get; set; }
        public string SetDate { get; set; }
        public Boolean AutoDate { get; set; }
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
