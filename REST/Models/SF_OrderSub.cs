using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class SF_OrderSub
    {
        [Key]
        [Required]
        public string OrderId { get; set; }
        [Required]
        public int i { get; set; }
        [Required]
        public string TableId { get; set; }
        [Required]
        public string FoodId { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; } // 1 = ยังไม่ได้ทำ, 2 = กำลังดำเนินการ, 3 = เสร็จแล้ว, 4 = ยกเลิกจากระบบครัว/ไม่มีวัตถุดิบ
        public int Options { get; set; } // 1 = ทานที่ร้าน, 2 = กลับบ้าน
        public string UserId { get; set; } // พ่อครัวที่เป็นคนทำ
        public string BranchId { get; set; }
    }
}
