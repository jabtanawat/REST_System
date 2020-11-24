using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class ViewMB_Member
    {
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string MemberId { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string DateRegister { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string IdCard { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string Title { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string TitleName { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string LastName { get; set; }
        public string Name { get; set; } // ชื่อเต็ม
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string DateBirthday { get; set; }
        public int Status { get; set; } // 1 = โสด, 2 = สมรส
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AddrNo { get; set; }
        public string Moo { get; set; }
        public string Road { get; set; }
        public string Soi { get; set; }
        public string Locality { get; set; } // ตำบล
        public string District { get; set; } // อำเภอ
        public string Province { get; set; }
        public string ZibCode { get; set; }
        public int Rebate { get; set; } // ส่วนลด
        public string Score { get; set; } // คะแนนสะสม

        public int Bch { get; set; } // 1 = ใช้ทุกสาขา, 2 = เลือกสาขา
        public string BchName { get; set; }
    }
}
