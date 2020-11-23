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
        public int Tax { get; set; } // 1 = ภาษี, 2 = ไม่คิดภาษี
        public decimal QtyLow { get; set; }
        public decimal QtyBalance { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string Unit { get; set; }
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
