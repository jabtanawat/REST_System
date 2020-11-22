using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewZone
    {
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string ZoneId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string ZoneName { get; set; }
        public string Description { get; set; }
        public string Front { get; set; }
        public string Number { get; set; }
        public Boolean AutoRun { get; set; }
        // -------------------------------------------------------
        public int Bch { get; set; } // 1 = ใช้ได้ทุกสาขา, 2 = เลือกสาขา
        public string BchName { get; set; }
    }
}
