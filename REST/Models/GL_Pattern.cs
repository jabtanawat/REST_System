﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class GL_Pattern
    {
        [Key]
        public string P_ID { get; set; }
        public string P_Name { get; set; }
        public string BookId { get; set; }
        public string MenuId { get; set; }
        public string Description { get; set; }
        // -------------------------------------------------------
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
