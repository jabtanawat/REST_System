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
using REST.Service;
using REST.ViewModels;
using static REST.Service.Enums;

namespace REST.Controllers
{
    [Authorize]
    public class RunningController : BaseController
    {
        #region db
        private readonly DbConnection _db;

        public RunningController(DbConnection db)
        {
            _db = db;
        }

        #endregion 
        public IActionResult Index()
        {
            var LOrder = GetRunning("Order");
            var IOrder = new ViewRunning();
            foreach (var row in LOrder)
            {
                IOrder.Name = row.Name;
                IOrder.Number = row.Number;
            }
            ViewBag.RunningOrder = IOrder;

            var LPayment = GetRunning("Payment");
            var IPayment = new ViewRunning();
            foreach (var row in LPayment)
            {
                IPayment.Name = row.Name;
                IPayment.Number = row.Number;
            }
            ViewBag.RunningPayment = IPayment;

            var LStore = GetRunning("Store");
            var IStore = new ViewRunning();
            foreach (var row in LStore)
            {
                IStore.Name = row.Name;
                IStore.Number = row.Number;
            }
            ViewBag.RunningStore = IStore;

            var LMember = GetRunning("Member");
            var IMember = new ViewRunning();
            foreach (var row in LMember)
            {
                IMember.Name = row.Name;
                IMember.Number = row.Number;
            }
            ViewBag.RunningMember = IMember;

            return View();
        }

        public IActionResult Edit(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Item = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == BranchId);
            return View(Item);
        }

        [HttpPost]
        public IActionResult Edit(CD_Running Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var DT = new DataTable();
            var sql = $"SELECT * FROM CD_Running WHERE Name = '{Results.Name}' AND BranchId = {BranchId}";
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
                if (UpdateData(Results, BranchId))
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
                if (SaveData(Results, BranchId))
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
        }

        public Boolean SaveData(CD_Running Results, string BranchId)
        {
            try
            {
                var Item = new CD_Running();
                Item.Name = Results.Name;
                Item.Front = Results.Front;
                Item.Number = Results.Number;
                Item.AutoRun = Results.AutoRun;
                Item.SetDate = Results.SetDate;
                Item.AutoDate = Results.AutoRun;
                Item.SetDate = Results.SetDate;
                Item.AutoDate = Results.AutoDate;
                /* DATA */
                Item.BranchId = BranchId;
                Item.CreateUser = User.Identity.Name;
                Item.CreateDate = DateTime.Now;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;

                _db.CD_Running.Add(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean UpdateData(CD_Running Results, string BranchId)
        {
            try
            {
                var Item = new CD_Running();
                Item = _db.CD_Running.FirstOrDefault(x => x.Name == Results.Name && x.BranchId == BranchId);
                Item.Front = Results.Front;
                Item.Number = Results.Number;
                Item.AutoRun = Results.AutoRun;
                Item.SetDate = Results.SetDate;
                Item.AutoDate = Results.AutoDate;
                /* DATA */
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;

                _db.CD_Running.Update(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Running
        public List<ViewRunning> GetRunning(string id)
        {
            var List = new List<ViewRunning>();
            string DocRunning = null;
            DateTime Date = Share.FormatDate(DateTime.Now).Date;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == BranchId);

            if (Running.AutoRun == true)
            {
                DocRunning = Running.Front + Running.Number;

                if (Running.AutoDate == true)
                {
                    DocRunning = Running.Front + Date.ToString(Running.SetDate) + Running.Number;
                }
            }

            var Item = new ViewRunning();
            Item.Name = Running.Name;
            Item.Number = DocRunning;

            List.Add(Item);

            return List;
        }

        public IActionResult Runnings(string id)
        {
            string DocRunning = null;
            string RunLength = null;
            int Number = 0;
            DateTime Date = Share.FormatDate(DateTime.Now).Date;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == BranchId);

            if (id != null)
            {
                if (Running.AutoRun == true)
                {
                    RunLength = null;
                    for (int i = 0; i < Running.Number.Length; i++)
                    {
                        RunLength += '0';
                    }
                    Number = Int32.Parse(Running.Number) + 1;
                    DocRunning = Running.Front + Number.ToString(RunLength);

                    if (Running.AutoDate == true)
                    {
                        RunLength = null;
                        for (int i = 0; i < Running.Number.Length; i++)
                        {
                            RunLength += '0';
                        }
                        Number = Int32.Parse(Running.Number) + 1;
                        DocRunning = Running.Front + Date.ToString(Running.SetDate) + Number.ToString(RunLength);
                    }
                }
            }

            return Json(new { data = DocRunning });
        }
    }
}
