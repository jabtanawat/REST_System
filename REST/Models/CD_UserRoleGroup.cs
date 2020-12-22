using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Models
{
    public class CD_UserRoleGroup
    {
        [Key]
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string RoleGroupId { get; set; }
        [Required(ErrorMessage = "กรุณากรอกข้อมูล")]
        public string RoleGroupName { get; set; }
        public Boolean SystemConfig { get; set; }
        public Boolean SaveData { get; set; }
        public Boolean ApproveData { get; set; }
        public Boolean ViewData { get; set; }
        public Boolean PrintReport { get; set; }
        public string Description { get; set; }
        public Boolean Active { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
