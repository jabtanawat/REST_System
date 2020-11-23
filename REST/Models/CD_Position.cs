using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class CD_Position
    {
        [Key]
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string PositionId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string PositionName { get; set; }
        public string Description { get; set; }
        // -------------------------------------------------------
        public int Bch { get; set; } // 1 = ใช้ได้ทุกสาขา, 2 = เลือกสาขา
        public string BchName { get; set; }
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
