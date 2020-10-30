using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class CD_Staple
    {
        [Key]
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string StapleId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string StapleName { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string UnitId { get; set; }
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
