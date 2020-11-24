using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class MB_Emproyee
    {
        [Key]
        public string EmproyeeId { get; set; }
        public string PositionId { get; set; }
        public DateTime DateRegister { get; set; }
        public string IdCard { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateBirthday { get; set; }
        public int Status { get; set; } // 1 = โสด, 2 = สมรส
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
