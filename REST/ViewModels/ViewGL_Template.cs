using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewGL_Template
    {
        [Key]
        public string M_ID { get; set; }
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string M_Name { get; set; }
        public string M_DesCription { get; set; }
        public string M_PType { get; set; }
        public List<ViewGL_TemplateSub> GL_TemplateSub { get; set; }
        public string Sub { get; set; }
    }
}
