using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class SF_Order
    {
        [Key]
        [Required]
        public string OrderId { get; set; }
        [Required]
        public string TableId { get; set; }
        public DateTime OrderDt { get; set; }
        public decimal PriceTotal { get; set; }
        public int ST { get; set; } // 1 = ยังไม่ได้ทำ, 2 = กำลังดำเนินการ, 3 = เสร็จแล้ว
        public int Success { get; set; } // 1 = ยังไม่ได้เช็คบิล, 2 = เช็คบิลแล้ว
        public string UserId { get; set; } // พ่อครัวที่เป็นคนทำ
        public string BranchId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }        
    }
}
