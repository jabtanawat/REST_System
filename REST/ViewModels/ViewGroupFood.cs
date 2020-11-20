using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewGroupFood
    {
        [Required]
        public string GroupFoodId { get; set; }
        [Required]
        public string GroupFoodName { get; set; }
        public string Description { get; set; }
        public string Front { get; set; }
        public string Number { get; set; }
        public Boolean AutoRun { get; set; }
        // -------------------------------------------------------
        public int Bch { get; set; } // 1 = ใช้ได้ทุกสาขา, 2 = เลือกสาขา
        public string BranchId { get; set; }
    }
}
