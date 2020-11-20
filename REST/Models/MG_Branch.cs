﻿using System;
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
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
