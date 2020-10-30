using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class DishController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public DishController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.DT_Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
            return View();
        }

        public IActionResult Insert()
        {
            return View();
        }

        public IActionResult Update(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Item = _db.CD_Dish.FirstOrDefault(x => x.DishId == id && x.BranchId == BranchId);

            return View(Item);
        }

        // ------

        public IActionResult Delete(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var DT = new DataTable();
            var sql = $"SELECT * FROM CD_Food WHERE BranchId = '{BranchId}' AND DishId = '{id}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    DT.Load(data);
                }
            }

            if (DT.Rows.Count > 0)
            {
                Alert("ไม่สามารถลบข้อมูลได้", "เนื่องจากมีข้อมูลอยู่ในทะเบียนวัตถุดิบ !", Enums.NotificationType.error);
                return RedirectToAction("Index");
            }
            else
            {
                if (DeleteData(id, BranchId))
                {
                    AlertTop("ลบข้อมูลเรียบร้อย", Enums.NotificationType.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    Alert("", "ไม่สามารถลบข้อมูลได้ !", Enums.NotificationType.error);
                    return RedirectToAction("Index");
                }              
            }
        }

        // ------

        [HttpPost]
        public IActionResult Insert(CD_Dish Results)
        {
            if (ModelState.IsValid)
            {
                var DT = new DataTable();
                var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId").Value;
                var sql = $"SELECT * FROM CD_Dish WHERE BranchId = '{BranchId}' AND DishId = '{Results.DishId}'";
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    _db.Database.OpenConnection();
                    using (var data = command.ExecuteReader())
                    {
                        DT.Load(data);
                    }
                }

                if (DT.Rows.Count > 0)
                {
                    Alert("", "รหัส นี้มีอยู่แล้ว !", Enums.NotificationType.error);
                    return View();
                }
                else
                {
                    if(SaveData(Results, BranchId))
                    {
                        AlertTop("บันทึกข้อมูลเรียบร้อย", Enums.NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", Enums.NotificationType.error);
                        return View();
                    }                  
                }
            }
            else
            {
                Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", Enums.NotificationType.error);
                return View();
            }
        }

        [HttpPost]
        public IActionResult Update(CD_Dish Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            if (ModelState.IsValid)
            {

                if (UpdateData(Results, BranchId))
                {
                    AlertTop("แก้ไขข้อมูลเรียบร้อย", Enums.NotificationType.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    Alert("", "ไม่สามารถแก้ไขข้อมูลได้ !", Enums.NotificationType.error);
                    return View();
                }                
            }
            else
            {
                Alert("", "ไม่สามารถแก้ไขข้อมูลได้ !", Enums.NotificationType.error);
                return View();
            }
        }

        // ------

        public Boolean SaveData(CD_Dish Results, string BranchId)
        {
            try
            {
                var Item = new CD_Dish();
                Item.DishId = Results.DishId;
                Item.DishName = Results.DishName;
                Item.Description = Results.Description;
                /* DATA */
                Item.BranchId = BranchId;
                Item.CreateUser = User.Identity.Name;
                Item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Dish.Add(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean UpdateData(CD_Dish Results, string BranchId)
        {
            try
            {
                var Item = new CD_Dish();
                Item = _db.CD_Dish.FirstOrDefault(x => x.DishId == Results.DishId && x.BranchId == BranchId);

                Item.DishName = Results.DishName;
                Item.Description = Results.Description;
                /* DATA */
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Dish.Update(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public Boolean DeleteData(string Id, string BranchId)
        {
            try
            {
                var Item = _db.CD_Dish.FirstOrDefault(x => x.DishId == Id && x.BranchId == BranchId);
                _db.CD_Dish.Remove(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }
    }
}
