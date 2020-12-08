using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewSF_OrderSub
    {
        public string OrderId { get; set; }
        public int i { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
        public string FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; } // 1 = ยังไม่ได้ทำ, 2 = กำลังดำเนินการ, 3 = เสร็จแล้ว
        public int Options { get; set; } // 1 = ทานที่ร้าน, 2 = กลับบ้าน
        public string BranchId { get; set; }
        public string Name { get; set; }
    }
}
