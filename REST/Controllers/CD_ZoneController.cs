using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;

namespace REST.Controllers
{
    public class CD_ZoneController : BaseController
    {

        #region Connect db
        private readonly DbConnection _db;

        public CD_ZoneController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        //------
        
        public IActionResult Index()
        {
            ViewBag.DT_Zone = GetZone();
            return View();
        }



        public IActionResult Insert()
        {
            return View();
        }

        public IActionResult Update(string id)
        {
            ViewBag.Id = id;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var Zone = _db.CD_Zone.FirstOrDefault(x => x.ZoneId == id && x.BranchId == BranchId);
            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == BranchId);

            var Item = new ViewZone();
            Item.ZoneId = Zone.ZoneId;
            Item.ZoneName = Zone.ZoneName;
            Item.Description = Zone.Description;
            Item.Front = Running.Front;
            Item.Number = Running.Number;
            Item.AutoRun = Running.AutoRun;

            return View(Item);
        }

        public IActionResult Delete(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var DT = new DataTable();
            var sql = $"SELECT * FROM CD_Table WHERE BranchId = '{BranchId}' AND ZoneId = '{id}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    DT.Load(data);
                }
            }

            if(DT.Rows.Count > 0)
            {
                Alert("", "ไม่สามารถลบได้ เนื่องจากมีข้อมูลอยู่ในทะเบียนโต๊ะ", Enums.NotificationType.warning);
                return RedirectToAction("Index");
            }
            else
            {
                if (DeleteData(id, BranchId))
                {
                    if (DeleteRunning(id, BranchId))
                    {
                        AlertTop("ลบข้อมูลเรียบร้อย", Enums.NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Alert("", "ไม่สามารถลบข้อมูล Running ได้", Enums.NotificationType.error);
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    Alert("", "ลบข้อมูลเรียบร้อย", Enums.NotificationType.error);
                    return RedirectToAction("Index");
                }
            }                       
        }

        // ------

        [HttpPost]
        public IActionResult Insert(ViewZone Results)
        {
            if (ModelState.IsValid)
            {
                var DT = new DataTable();
                var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId").Value;
                var sql = $"SELECT * FROM CD_Zone WHERE BranchId = '{BranchId}' AND ZoneId = '{Results.ZoneId}'";
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
                    // Save Zone
                    if(SaveData(Results, BranchId))
                    {
                        // Save Running
                        if (SaveRunning(Results, BranchId))
                        {
                            AlertTop("บันทึกข้อมูลเรียบร้อย", Enums.NotificationType.success);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Alert("", "ไม่สามารถบันทึกข้อมูล Running ได้ !", Enums.NotificationType.error);
                            return View();
                        }                        
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
        public IActionResult Update(ViewZone Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            if (ModelState.IsValid)
            {
                // Update Zone
                if(UpdateData(Results, BranchId))
                {
                    // Save Running
                    if (SaveRunning(Results, BranchId))
                    {
                        AlertTop("แก้ไขข้อมูลเรียบร้อย", Enums.NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Alert("", "ไม่สามารถบันทึกข้อมูล Running ได้ !", Enums.NotificationType.error);
                        return View();
                    }                    
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

        public Boolean SaveData(ViewZone Results, string BranchId)
        {
            try
            {
                var Item = new CD_Zone();
                Item.ZoneId = Results.ZoneId;
                Item.ZoneName = Results.ZoneName;
                Item.Description = Results.Description;
                /* DATA */
                Item.BranchId = BranchId;
                Item.CreateUser = User.Identity.Name;
                Item.CreateDate = DateTime.Now;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;

                _db.CD_Zone.Add(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public Boolean UpdateData(ViewZone Results, string BranchId)
        {
            try
            {
                var Item = new CD_Zone();
                Item = _db.CD_Zone.FirstOrDefault(x => x.ZoneId == Results.ZoneId && x.BranchId == BranchId);

                Item.ZoneName = Results.ZoneName;
                Item.Description = Results.Description;
                /* DATA */
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;

                _db.CD_Zone.Update(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public Boolean DeleteData(string id, string BranchId)
        {
            try
            {
                var Item = _db.CD_Zone.FirstOrDefault(x => x.ZoneId == id && x.BranchId == BranchId);
                _db.CD_Zone.Remove(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public Boolean SaveRunning(ViewZone Results, string BranchId)
        {
            try
            {
                var DT = new DataTable();
                var sql = $"SELECT * FROM CD_Running WHERE Name = '{Results.ZoneId}' AND BranchId = {BranchId}";
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
                    Item = _db.CD_Running.FirstOrDefault(x => x.Name == Results.ZoneId && x.BranchId == BranchId);
                    Item.Name = Results.ZoneId;
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
                    Item.Name = Results.ZoneId;
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

        public Boolean DeleteRunning(string Id, string BranchId)
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

        // ------

        public List<CD_Zone> GetZone()
        {
            var List = new List<CD_Zone>();
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT ZoneId, ZoneName, Description FROM CD_Zone "
                    + $"WHERE BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new CD_Zone();
                        Item.ZoneId = data.GetString(0);
                        Item.ZoneName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Description = data.GetString(2);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }
    }
}
