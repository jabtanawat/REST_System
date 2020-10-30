using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewST_TranManual
    {
        public string StapleName { get; set; }
        public string Trans { get; set; }
        public DateTime Dates { get; set; }
        public string Times { get; set; }
        public decimal After { get; set; }
        public decimal Edit { get; set; }
        public decimal Balance { get; set; }
    }
}
