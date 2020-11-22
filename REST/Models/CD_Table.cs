using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class CD_Table
    {
        [Key]
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string TableId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string TableName { get; set; }
        public int Personal { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public int TableST { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string ZoneId { get; set; }
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
