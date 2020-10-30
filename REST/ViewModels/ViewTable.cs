using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewTable
    {
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string ZoneId { get; set; }
        public string ZoneName { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string TableId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string TableName { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public int Personal { get; set; }
        public string Description { get; set; }
        public int TableST { get; set; }
        public string Status { get; set; }
    }
}
