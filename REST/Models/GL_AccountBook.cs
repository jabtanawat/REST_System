﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class GL_AccountBook
    {
        [Key]
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string BookId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string BookName { get; set; }
        // -------------------------------------------------------
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
