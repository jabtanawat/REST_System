using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewCD_Drink
    {
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string DrinkId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string DrinkName { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string Price { get; set; }
        public string GroupFoodId { get; set; }
        public string ImageName { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public int Bch { get; set; }
        public string BchName { get; set; }
    }
}
