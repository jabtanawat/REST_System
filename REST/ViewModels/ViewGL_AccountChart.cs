using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewGL_AccountChart
    {
        public string Type { get; set; }
        public string Control { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string AccNo { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string AccName { get; set; }
        public string DrCr { get; set; }
    }
}
