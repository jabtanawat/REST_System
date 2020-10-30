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
    public class TableController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public TableController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        // ------

        public IActionResult Index()
        {
            ViewBag.DT_Table = GetTable();
            return View();
        }

        public IActionResult Insert()
        {
            ViewBag.SL_Zone = SL_Zone();
            return View();
        }

        public IActionResult Update(string Id)
        {
            ViewBag.Id = Id;
            ViewBag.SL_Zone = SL_Zone();
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Item = _db.CD_Table.FirstOrDefault(x => x.TableId == Id && x.BranchId == BranchId);
           
            return View(Item);
        }

        public IActionResult Delete(string Id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            if (DeleteData(Id, BranchId))
            {
                AlertTop("ลบข้อมูลเรียบร้อย", NotificationType.success);
                return RedirectToAction("Index", "Table");
            }
            else
            {
                Alert("", "ไม่สามารถลบข้อมูลได้ !", NotificationType.error);
                return View();
            }            
        }

        // ------

        [HttpPost]
        public IActionResult Insert(CD_Table Results)
        {
            if (ModelState.IsValid)
            {
                var DT = new DataTable();
                var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId").Value;
                var sql = $"SELECT * FROM CD_Table WHERE BranchId = '{BranchId}' AND TableId = '{Results.TableId}'";
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
                    Alert("", "รหัส นี้มีอยู่แล้ว !", NotificationType.error);
                    ViewBag.SL_Zone = SL_Zone();
                    return View();
                }
                else
                {
                    // Save Table
                    if (SaveData(Results, BranchId))
                    {
                        // Save Running
                        if(SetRunning(Results, BranchId))
                        {
                            AlertTop("บันทึกข้อมูลเรียบร้อย", NotificationType.success);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Alert("", "ไม่สามารถบันทึก Running ได้ !", NotificationType.error);
                            ViewBag.SL_Zone = SL_Zone();
                            return View();
                        }                        
                    }
                    else
                    {
                        Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", NotificationType.error);
                        ViewBag.SL_Zone = SL_Zone();
                        return View();
                    }                
                }
            }
            else
            {
                Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", NotificationType.error);
                ViewBag.SL_Zone = SL_Zone();
                return View();
            }
        }

        [HttpPost]
        public IActionResult Update(CD_Table Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            if (ModelState.IsValid)
            {         
                if(UpdateData(Results, BranchId))
                {
                    AlertTop("แก้ไขข้อมูลเรียบร้อย", NotificationType.success);
                    return RedirectToAction("Index", "Table");
                }
                else
                {
                    Alert("", "ไม่สามารถแก้ไขข้อมูลได้ !", NotificationType.error);
                    ViewBag.SL_Zone = SL_Zone();
                    return View();
                }                
            }
            else
            {
                Alert("", "ไม่สามารถแก้ไขข้อมูลได้ !", NotificationType.error);
                ViewBag.SL_Zone = SL_Zone();
                return View();
            }
        }        

        // ------        

        public Boolean SaveData(CD_Table Results, string BranchId)
        {
            try
            {
                var Item = new CD_Table();
                Item.TableId = Results.TableId;
                Item.TableName = Results.TableName;
                Item.Personal = Results.Personal;
                Item.Description = Results.Description;
                Item.TableST = Results.TableST;
                Item.ZoneId = Results.ZoneId;
                /* DATA */
                Item.BranchId = BranchId;
                Item.CreateUser = User.Identity.Name;
                Item.CreateDate = DateTime.Now;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;

                _db.CD_Table.Add(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public Boolean UpdateData(CD_Table Results, string BranchId)
        {
            try
            {
                var Item = new CD_Table();
                Item = _db.CD_Table.FirstOrDefault(x => x.TableId == Results.TableId && x.BranchId == BranchId);

                Item.TableName = Results.TableName;
                Item.Personal = Results.Personal;
                Item.Description = Results.Description;
                Item.TableST = Results.TableST;
                /* DATA */
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;

                _db.CD_Table.Update(Item);
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
                var Item = _db.CD_Table.FirstOrDefault(x => x.TableId == Id && x.BranchId == BranchId);
                _db.CD_Table.Remove(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        // ------

        public List<CD_Zone> SL_Zone()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<CD_Zone>();
            var sql = $"SELECT ZoneId, ZoneName FROM CD_Zone "
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
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewTable> GetTable()
        {
            var List = new List<ViewTable>();
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT TableId, TableName, Personal,  CASE WHEN TableST = 1 THEN 'ว่าง' WHEN TableST = 2 THEN 'ไม่ว่าง' WHEN TableST= 3 THEN 'จอง' END AS Status, ZoneName "
                    + $"FROM CD_Table "
                    + $"LEFT JOIN CD_Zone ON CD_Table.ZoneId = CD_Zone.ZoneId "
                    + $"WHERE CD_Table.BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewTable();
                        Item.TableId = data.GetString(0);
                        Item.TableName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Personal = data.GetInt32(2);
                        Item.Status = data.GetString(3);
                        Item.ZoneName = data.GetString(4);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public string GetRunning(string id)
        {
            string DocRunning = null;
            string RunLength = null;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == BranchId);
            if (id != null)
            {
                if (Running.AutoRun == true)
                {
                    for (int i = 0; i < Running.Number.Length; i++)
                    {
                        RunLength += '0';
                    }
                    int Number = Int32.Parse(Running.Number) + 1;
                    DocRunning = Running.Front + Number.ToString(RunLength);
                }
            }

            return DocRunning;
        }

        public Boolean SetRunning(CD_Table Results, string BranchId)
        {
            var Running = new CD_Running();
            Running = _db.CD_Running.FirstOrDefault(x => x.Name == Results.ZoneId && x.BranchId == BranchId);

            try
            {
                if (Running.AutoRun == true)
                {
                    int RunLength = Running.Number.Length;
                    Running.Number = Results.TableId.Substring(Results.TableId.Length - RunLength);

                    /* DATA */
                    Running.UpdateUser = User.Identity.Name;
                    Running.UpdateDate = DateTime.Now;

                    /* SAVE DB */
                    _db.CD_Running.Update(Running);
                    _db.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
