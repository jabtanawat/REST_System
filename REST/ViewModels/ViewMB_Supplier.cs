using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class ViewMB_Supplier
    {
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string SupplierId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string DateRegister { get; set; }
        public string TaxNumber { get; set; }
        public string IdCard { get; set; }
        public string Title { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; } // ชื่อเต็ม

        public int Bch { get; set; }
        public string BchName { get; set; }


        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string Email { get; set; }

        public string AddrNo { get; set; }
        public string Moo { get; set; }
        public string Road { get; set; }
        public string Soi { get; set; }
        public string Locality { get; set; } // ตำบล
        public string District { get; set; } // อำเภอ
        public string Province { get; set; }
        public string ZibCode { get; set; }
    }
}
