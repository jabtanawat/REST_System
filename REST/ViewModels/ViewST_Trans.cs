using REST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewST_Trans
    {
        public string Document { get; set; }
        public string DateDocument { get; set; }
        public string DateTax { get; set; }
        public string TaxNumber { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Reference { get; set; }
        public string SumBalance { get; set; }
        public string Sub { get; set; }
        public List<ViewST_TranSub> TranSub { get; set; }
    }
}
