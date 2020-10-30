using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;
using REST.ViewModels;
using static REST.Service.Enums;

namespace REST.Controllers
{
    [Authorize]
    public class GroupFoodController : BaseController
    {
        #region db
        private readonly DbConnection _db;

        public GroupFoodController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            ViewBag.DT_GroupFood = GetGroupFood();
            return View();
        }

        public IActionResult Insert()
        {
            return View();
        }

        public IActionResult Update(string Id)
        {
            ViewBag.Id = Id;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var GroupFood = _db.CD_GroupFood.FirstOrDefault(x => x.GroupFoodId == Id && x.BranchId == BranchId);
            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == Id && x.BranchId == BranchId);

            var Item = new ViewGroupFood();
            Item.GroupFoodId = GroupFood.GroupFoodId;
            Item.GroupFoodName = GroupFood.GroupFoodName;
            Item.Description = GroupFood.Description;
            Item.Front = Running.Front;
            Item.Number = Running.Number;
            Item.AutoRun = Running.AutoRun;

            return View(Item);
        }

        public IActionResult Delete(string Id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var DT = new DataTable();
            var sql = $"SELECT * FROM CD_Food WHERE BranchId = '{BranchId}' AND GroupFoodId = '{Id}'";
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
                Alert("ไม่สามารถลบข้อมูลได้", "เนื่องจากมีข้อมูลอยู่ในทะเบียนอาหาร !", NotificationType.error);
                return RedirectToAction("Update", new { Id = Id });
            }
            else
            {
                if (DeleteGroupFood(Id, BranchId))
                {
                    if (DeleteRunning(Id, BranchId))
                    {
                        AlertTop("ลบข้อมูลเรียบร้อย", NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AlertTop("ไม่สามารถลบข้อมูล Running ได้", NotificationType.error);
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    AlertTop("ไม่สามารถลบข้อมูลกลุ่มอาหารได้", NotificationType.error);
                    return RedirectToAction("Index");
                }
            }            
        }

        [HttpPost]
        public IActionResult Insert(ViewGroupFood Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            if (ModelState.IsValid)
            {
                var DT = new DataTable();                
                var sql = $"SELECT * FROM CD_GroupFood WHERE GroupFoodId = '{Results.GroupFoodId}' AND BranchId = '{BranchId}'";
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
                    Alert("Error", "รหัส นี้มีอยู่แล้ว !", NotificationType.error);
                    return View();
                }
                else
                {
                    // INSERT GROUPFOOD
                    if (SaveGroupFood(Results, BranchId))
                    {
                        // SAVE RUNNING NUMBER
                        if(SaveRunning(Results, BranchId))
                        {
                            AlertTop("บันทึกข้อมูลเรียบร้อย", NotificationType.success);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Alert("Error", "ไม่สามารถบันทึกข้อมูล Running ได้ !", NotificationType.error);
                            return View();
                        }
                    }
                    else
                    {
                        Alert("Error", "ไม่สามารถบันทึกข้อมูลกลุ่มอาหารได้ !", NotificationType.error);
                        return View();
                    }              
                }
            }
            else
            {
                Alert("Error", "ไม่สามารถบันทึกข้อมูลได้ !", NotificationType.error);
                return View();
            }
        }

        [HttpPost]
        public IActionResult Update(ViewGroupFood Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            if (ModelState.IsValid)
            {
                // UPDATE GROUPFOOD
                if (UpdateGroupFood(Results, BranchId))
                {
                    // SAVE RUNNING NUMBER
                    if (SaveRunning(Results, BranchId))
                    {
                        AlertTop("แก้ไขข้อมูลเรียบร้อย", NotificationType.success);
                        return RedirectToAction("Index", "GroupFood");
                    }
                    else
                    {
                        Alert("Error", "ไม่สามารถบันทึกข้อมูล Running ได้ !", NotificationType.error);
                        return View();
                    }                 
                }
                else
                {
                    Alert("Error", "ไม่สามารถบันทึกข้อมูลกลุ่มอาหารได้ !", NotificationType.error);
                    return View();
                }
            }
            else
            {
                Alert("Error", "ไม่สามารถแก้ไขข้อมูลได้ !", NotificationType.error);
                return View();
            }
        }

        private List<CD_GroupFood> GetGroupFood()
        {
            var List = new List<CD_GroupFood>();
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT GroupFoodId, GroupFoodName, Description "
                    + $"FROM CD_GroupFood "
                    + $"WHERE BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new CD_GroupFood();
                        Item.GroupFoodId = data.GetString(0);
                        Item.GroupFoodName = data.GetString(1);
                        if (!data.IsDBNull(2)) 
                        {
                            Item.Description = data.GetString(2);
                        } 
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        private Boolean SaveGroupFood(ViewGroupFood Results, string BranchId)
        {
            try
            {
                var Item = new CD_GroupFood();
                Item.GroupFoodId = Results.GroupFoodId;
                Item.GroupFoodName = Results.GroupFoodName;
                Item.Description = Results.Description;
                /* DATA */
                Item.BranchId = BranchId;
                Item.CreateUser = User.Identity.Name;
                Item.CreateDate = DateTime.Now;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;

                _db.CD_GroupFood.Add(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        private Boolean UpdateGroupFood(ViewGroupFood Results, string BranchId)
        {
            try
            {
                var Item = new CD_GroupFood();
                Item = _db.CD_GroupFood.FirstOrDefault(x => x.GroupFoodId == Results.GroupFoodId && x.BranchId == BranchId);

                Item.GroupFoodName = Results.GroupFoodName;
                Item.Description = Results.Description;
                /* DATA */
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;

                _db.CD_GroupFood.Update(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Boolean DeleteGroupFood(string Id, string BranchId)
        {
            try
            {
                var Item = _db.CD_GroupFood.FirstOrDefault(x => x.GroupFoodId == Id && x.BranchId == BranchId);
                _db.CD_GroupFood.Remove(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        private Boolean SaveRunning(ViewGroupFood Results, string BranchId)
        {
            try
            {
                var DT = new DataTable();
                var sql = $"SELECT * FROM CD_Running WHERE Name = '{Results.GroupFoodId}' AND BranchId = {BranchId}";
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
                    var Item = new CD_Running();
                    Item = _db.CD_Running.FirstOrDefault(x => x.Name == Results.GroupFoodId && x.BranchId == BranchId);
                    Item.Name = Results.GroupFoodId;
                    Item.Front = Results.Front;
                    Item.Number = Results.Number;
                    Item.AutoRun = Results.AutoRun;
                    /* DATA */
                    Item.UpdateUser = User.Identity.Name;
                    Item.UpdateDate = DateTime.Now;

                    _db.CD_Running.Update(Item);
                    _db.SaveChanges();
                }
                else
                {
                    var Item = new CD_Running();
                    Item.Name = Results.GroupFoodId;
                    Item.Front = Results.Front;
                    Item.Number = Results.Number;
                    Item.AutoRun = Results.AutoRun;
                    Item.SetDate = null;
                    Item.AutoDate = false;
                    /* DATA */
                    Item.BranchId = BranchId;
                    Item.CreateUser = User.Identity.Name;
                    Item.CreateDate = DateTime.Now;
                    Item.UpdateUser = User.Identity.Name;
                    Item.UpdateDate = DateTime.Now;

                    _db.CD_Running.Add(Item);
                    _db.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Boolean DeleteRunning(string Id, string BranchId)
        {
            try
            {
                var Item = _db.CD_Running.FirstOrDefault(x => x.Name == Id && x.BranchId == BranchId);
                _db.CD_Running.Remove(Item);
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
