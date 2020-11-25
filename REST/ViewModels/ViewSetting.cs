﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewSetting
    {
        public decimal Tax { get; set; }
        public decimal Service { get; set; }
        public decimal Point { get; set; }
        public string PointNumber { get; set; }
        public Boolean Perbunch { get; set; }
    }
}
