using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewRunning
    {
        public string Name { get; set; }
        public string NameTh { get; set; }
        public string Front { get; set; }
        public string Number { get; set; }
        public Boolean AutoRun { get; set; }
        public string SetDate { get; set; }
        public Boolean AutoDate { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
    }
}
