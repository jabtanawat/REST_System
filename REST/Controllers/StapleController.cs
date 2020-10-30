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
    public class StapleController : BaseController
    {
        #region db
        private readonly DbConnection _db;

        public StapleController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            ViewBag.DT_Staple = GetStaple();
            return View();
        }

        public IActionResult Insert()
        {
            ViewBag.SL_Unit = GetUnit(); 
            return View();
        }

        public IActionResult Update(string Id)
        {
            ViewBag.Id = Id;
            ViewBag.SL_Unit = GetUnit();
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == Id && x.BranchId == BranchId);

            return View(Item);
        }

        public IActionResult Delete(string Id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            DeleteStaple(Id, BranchId);

            AlertTop("ลบข้อมูลเรียบร้อย", NotificationType.success);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Insert(CD_Staple Results)
        {
            if (ModelState.IsValid)
            {
                var DT = new DataTable();
                var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId").Value;
                var sql = $"SELECT * FROM CD_Staple WHERE BranchId = '{BranchId}' AND StapleId = '{Results.StapleId}'";
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
                    ViewBag.SL_Unit = GetUnit();
                    return View();
                }
                else
                {
                    SaveStaple(Results, BranchId);

                    AlertTop("บันทึกข้อมูลเรียบร้อย", NotificationType.success);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Alert("Error", "ไม่สามารถบันทึกข้อมูลได้ !", NotificationType.error);
                ViewBag.SL_Unit = GetUnit();
                return View();
            }
        }

        [HttpPost]
        public IActionResult Update(CD_Staple Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            if (ModelState.IsValid)
            {

                UpdateStaple(Results, BranchId);

                AlertTop("แก้ไขข้อมูลเรียบร้อย", NotificationType.success);
                return RedirectToAction("Index");
            }
            else
            {
                Alert("Error", "ไม่สามารถแก้ไขข้อมูลได้ !", NotificationType.error);
                ViewBag.SL_Unit = GetUnit();
                return View();
            }
        }

        public List<ViewStaple> GetStaple()
        {
            var List = new List<ViewStaple>();
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT StapleId, StapleName, Amount, UnitName "
                    + $"FROM CD_Staple "
                    + $"LEFT JOIN CD_Unit ON CD_Staple.UnitId = CD_Unit.UnitId "
                    + $"WHERE CD_Staple.BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewStaple();
                        Item.StapleId = data.GetString(0);
                        Item.StapleName = data.GetString(1);
                        Item.Amount = data.GetDecimal(2);
                        Item.UnitName = data.GetString(3);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<CD_Unit> GetUnit()
        {
            var List = new List<CD_Unit>();
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT UnitId, UnitName "
                    + $"FROM CD_Unit "
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
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public void SaveStaple(CD_Staple Results, string BranchId)
        {
            var Item = new CD_Staple();
            Item.StapleId = Results.StapleId;
            Item.StapleName = Results.StapleName;
            Item.Amount = Results.Amount;
            Item.UnitId = Results.UnitId;
            /* DATA */
            Item.BranchId = BranchId;
            Item.CreateUser = User.Identity.Name;
            Item.CreateDate = DateTime.Now;
            Item.UpdateUser = User.Identity.Name;
            Item.UpdateDate = DateTime.Now;

            _db.CD_Staple.Add(Item);
            _db.SaveChanges();

            return;
        }

        public void UpdateStaple(CD_Staple Results, string BranchId)
        {
            var Item = new CD_Staple();
            Item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == Results.StapleId && x.BranchId == BranchId);

            Item.StapleName = Results.StapleName;
            Item.Amount = Results.Amount;
            Item.UnitId = Results.UnitId;
            /* DATA */
            Item.UpdateUser = User.Identity.Name;
            Item.UpdateDate = DateTime.Now;

            _db.CD_Staple.Update(Item);
            _db.SaveChanges();

            return;
        }

        public void DeleteStaple(string Id, string BranchId)
        {
            var Item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == Id && x.BranchId == BranchId);
            _db.CD_Staple.Remove(Item);
            _db.SaveChanges();

            return;
        }
    }
}
