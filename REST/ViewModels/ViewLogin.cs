using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewLogin
    {
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public String UserName { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public String PassWord { get; set; }
    }
}
