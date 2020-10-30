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
using static REST.Service.Enums;

namespace REST.Controllers
{
    [Authorize]
    public class UnitController : BaseController
    {
        #region db
        private readonly DbConnection _db;

        public UnitController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            ViewBag.DT_Unit = GetUnit();
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
            var Item = _db.CD_Unit.FirstOrDefault(x => x.UnitId == Id && x.BranchId == BranchId);

            return View(Item);
        }

        public IActionResult Delete(string Id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var DT = new DataTable();
            var sql = $"SELECT * FROM CD_Staple WHERE BranchId = '{BranchId}' AND UnitId = '{Id}'";
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
                Alert("ไม่สามารถลบข้อมูลได้", "เนื่องจากมีข้อมูลอยู่ในทะเบียนวัตถุดิบ !", NotificationType.error);
                return RedirectToAction("Update", new { Id = Id});
            }
            else
            {
                DeleteUnit(Id, BranchId);

                AlertTop("ลบข้อมูลเรียบร้อย", NotificationType.success);
                return RedirectToAction("Index");
            }            
        }

        [HttpPost]
        public IActionResult Insert(CD_Unit Results)
        {
            if (ModelState.IsValid)
            {
                var DT = new DataTable();
                var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId").Value;
                var sql = $"SELECT * FROM CD_Unit WHERE BranchId = '{BranchId}' AND UnitId = '{Results.UnitId}'";
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
                    SaveUnit(Results, BranchId);

                    AlertTop("บันทึกข้อมูลเรียบร้อย", NotificationType.success);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Alert("Error", "ไม่สามารถบันทึกข้อมูลได้ !", NotificationType.error);
                return View();
            }
        }

        [HttpPost]
        public IActionResult Update(CD_Unit Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            if (ModelState.IsValid)
            {

                UpdateUnit(Results, BranchId);

                AlertTop("แก้ไขข้อมูลเรียบร้อย", NotificationType.success);
                return RedirectToAction("Index");
            }
            else
            {
                Alert("Error", "ไม่สามารถแก้ไขข้อมูลได้ !", NotificationType.error);
                return View();
            }
        }

        public List<CD_Unit> GetUnit()
        {
            var List = new List<CD_Unit>();
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT UnitId, UnitName, Description FROM CD_Unit "
                    + $"WHERE BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new CD_Unit();
                        Item.UnitId = data.GetString(0);
                        Item.UnitName = data.GetString(1);
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

        public void SaveUnit(CD_Unit Results, string BranchId)
        {

            var Item = new CD_Unit();
            Item.UnitId = Results.UnitId;
            Item.UnitName = Results.UnitName;
            Item.Description = Results.Description;
            /* DATA */
            Item.BranchId = BranchId;
            Item.CreateUser = User.Identity.Name;
            Item.CreateDate = DateTime.Now;
            Item.UpdateUser = User.Identity.Name;
            Item.UpdateDate = DateTime.Now;

            _db.CD_Unit.Add(Item);
            _db.SaveChanges();

            return;
        }

        public void UpdateUnit(CD_Unit Results, string BranchId)
        {
            var Item = new CD_Unit();
            Item = _db.CD_Unit.FirstOrDefault(x => x.UnitId == Results.UnitId && x.BranchId == BranchId);

            Item.UnitName = Results.UnitName;
            Item.Description = Results.Description;
            /* DATA */
            Item.UpdateUser = User.Identity.Name;
            Item.UpdateDate = DateTime.Now;

            _db.CD_Unit.Update(Item);
            _db.SaveChanges();

            return;
        }

        public void DeleteUnit(string Id, string BranchId)
        {
            var Item = _db.CD_Unit.FirstOrDefault(x => x.UnitId == Id && x.BranchId == BranchId);
            _db.CD_Unit.Remove(Item);
            _db.SaveChanges();

            return;
        }
    }
}
