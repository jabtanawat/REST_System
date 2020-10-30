using REST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewST_Trans
    {
        public string Documents { get; set; }
        public string Dates { get; set; }
        public string Description { get; set; }
        public List<ViewST_TranSub> TranSub { get; set; }
    }
}
