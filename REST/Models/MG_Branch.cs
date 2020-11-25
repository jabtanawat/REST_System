using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace REST.Models
{
    public class MG_Branch
    {
        [Key]
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string BranchId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string BranchName { get; set; }
        public Boolean St { get; set; }
        public string AddrNo { get; set; }
        public string Moo { get; set; }
        public string Road { get; set; }
        public string Soi { get; set; }
        public string Locality { get; set; } // ตำบล
        public string District { get; set; } // อำเภอ
        public string Province { get; set; }
        public string ZibCode { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
