using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewGL_Pattern
    {
        public string P_ID { get; set; }
        public string P_Name { get; set; }
        public string BookId { get; set; }    
        public string MenuId { get; set; }
        public string Description { get; set; }
        public List<ViewGL_PatternSub> GL_PatternSub { get; set; }
        public string Sub { get; set; }
    }
}
