using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ViewModels
{
    public class ViewFood
    {
        public string FoodId { get; set; }
        public string FoodName { get; set; }
        public string Price { get; set; }
        public string GroupFoodId { get; set; }
        public string GroupFoodName { get; set; }
        public string DishId { get; set; }
        public string DishName { get; set; }
        public string ImageName { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public string Sub { get; set; }
        public List<ViewFoodSub> FoodSub { get; set; }
        public int Bch { get; set; }
        public string BchName { get; set; }

    }
}
