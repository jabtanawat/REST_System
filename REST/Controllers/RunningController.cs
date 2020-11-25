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
        #region Connect db
        private readonly DbConnection _db;

        public RunningController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult Index()
        {
            LoadForm();
            return View();            
        }

        public IActionResult FrmRunning(string name, string branchid)
        {
            var item = new ViewRunning();
            var infoBranch = _db.MG_Branch.FirstOrDefault(x => x.BranchId == branchid);
            var infoRunning = _db.CD_Running.FirstOrDefault(x => x.Name == name && x.BranchId == branchid);
            item.BranchId = infoBranch.BranchId;
            item.BranchName = infoBranch.BranchName;
            item.Name = infoRunning.Name;
            if (infoRunning.Name == "Emproyee")
                item.NameTh = "พนักงาน";
            else if (infoRunning.Name == "Member")
                item.NameTh = "สมาชิก";
            item.Front = infoRunning.Front;
            item.Number = infoRunning.Number;
            item.AutoRun = infoRunning.AutoRun;
            item.SetDate = infoRunning.SetDate;
            item.AutoDate = infoRunning.AutoDate;
            return View(item);
        }

        [HttpPost]
        public IActionResult FrmRunning(ViewRunning info)
        {
            if (ModelState.IsValid)
            {
                if (SaveData(info))
                {
                    toastrAlert("เลข Running", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", Enums.NotificationType.error);
                return View();
            }
        }

        public JsonResult AView(string id)
        {
            var List = new List<ViewRunning>();

            var LEmproyee = _db.CD_Running.Where(x => x.Name == "Emproyee" && x.BranchId == id).ToList();
            var IEmproyee = new ViewRunning();
            foreach (var row in LEmproyee)
            {
                IEmproyee.Name = row.Name;
                IEmproyee.NameTh = "พนักงาน";
                IEmproyee.Front = row.Front;
                IEmproyee.Number = row.Number;
                IEmproyee.AutoRun = row.AutoRun;
                List.Add(IEmproyee);
            }

            var LMember = _db.CD_Running.Where(x => x.Name == "Member" && x.BranchId == id).ToList();
            var IMember = new ViewRunning();
            foreach (var row in LMember)
            {
                IMember.Name = row.Name;
                IMember.NameTh = "สมาชิก";
                IMember.Front = row.Front;
                IMember.Number = row.Number;
                IMember.AutoRun = row.AutoRun;
                List.Add(IMember);
            }

            var LOrder = _db.CD_Running.Where(x => x.Name == "Order" && x.BranchId == id).ToList();
            var IOrder = new ViewRunning();
            foreach (var row in LOrder)
            {
                IOrder.Name = row.Name;
                IOrder.NameTh = "ออเดอร์";
                IOrder.Front = row.Front;
                IOrder.Number = row.Number;
                IOrder.AutoRun = row.AutoRun;
                List.Add(IOrder);
            }

            var LPayment = _db.CD_Running.Where(x => x.Name == "Payment" && x.BranchId == id).ToList();
            var IPayment = new ViewRunning();
            foreach (var row in LPayment)
            {
                IPayment.Name = row.Name;
                IPayment.NameTh = "ชำระเงิน";
                IPayment.Front = row.Front;
                IPayment.Number = row.Number;
                IPayment.AutoRun = row.AutoRun;
                List.Add(IPayment);
            }

            var LStore = _db.CD_Running.Where(x => x.Name == "Store" && x.BranchId == id).ToList();
            var IStore = new ViewRunning();
            foreach (var row in LStore)
            {
                IStore.Name = row.Name;
                IStore.NameTh = "สั่งซื้อ";
                IStore.Front = row.Front;
                IStore.Number = row.Number;
                IStore.AutoRun = row.AutoRun;
                List.Add(IStore);
            }

            return Json(new { data = List }) ;
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
                //if (SaveData(Results))
                //{
                //    AlertTop("บันทึกข้อมูลเรียบร้อย", NotificationType.success);
                //    return RedirectToAction("Index");
                //}
                //else
                //{
                //    Alert("Error", "ไม่สามารถบันทึกข้อมูล Running ได้ !", NotificationType.error);
                //    return View();
                //}
                return View();
            }
        }

        public Boolean SaveData(ViewRunning info)
        {
            var item = new CD_Running();
            try
            {
                item = _db.CD_Running.FirstOrDefault(x => x.Name == info.Name && x.BranchId == info.BranchId);
                item.Front = info.Front;
                item.Number = info.Number;
                item.AutoRun = info.AutoRun;
                item.SetDate = info.SetDate;
                item.AutoDate = info.AutoRun;
                /* DATA */
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Running.Update(item);
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

        public void LoadForm()
        {
            var List = new List<ViewRunning>();
            var B = _db.MG_Branch.FirstOrDefault();

            var LEmproyee = _db.CD_Running.Where(x => x.Name == "Emproyee" && x.BranchId == B.BranchId).ToList();
            var IEmproyee = new ViewRunning();
            foreach (var row in LEmproyee)
            {
                IEmproyee.Name = row.Name;
                IEmproyee.NameTh = "พนักงาน";
                IEmproyee.Front = row.Front;
                IEmproyee.Number = row.Number;
                IEmproyee.AutoRun = row.AutoRun;
                List.Add(IEmproyee);
            }

            var LMember = _db.CD_Running.Where(x => x.Name == "Member" && x.BranchId == B.BranchId).ToList();
            var IMember = new ViewRunning();
            foreach (var row in LMember)
            {
                IMember.Name = row.Name;
                IMember.NameTh = "สมาชิก";
                IMember.Front = row.Front;
                IMember.Number = row.Number;
                IMember.AutoRun = row.AutoRun;
                List.Add(IMember);
            }

            var LOrder = _db.CD_Running.Where(x => x.Name == "Order" && x.BranchId == B.BranchId).ToList();
            var IOrder = new ViewRunning();
            foreach (var row in LOrder)
            {
                IOrder.Name = row.Name;
                IOrder.NameTh = "ออเดอร์";
                IOrder.Front = row.Front;
                IOrder.Number = row.Number;
                IOrder.AutoRun = row.AutoRun;
                List.Add(IOrder);
            }

            var LPayment = _db.CD_Running.Where(x => x.Name == "Payment" && x.BranchId == B.BranchId).ToList();
            var IPayment = new ViewRunning();
            foreach (var row in LPayment)
            {
                IPayment.Name = row.Name;
                IPayment.NameTh = "ชำระเงิน";
                IPayment.Front = row.Front;
                IPayment.Number = row.Number;
                IPayment.AutoRun = row.AutoRun;
                List.Add(IPayment);
            }

            var LStore = _db.CD_Running.Where(x => x.Name == "Store" && x.BranchId == B.BranchId).ToList();
            var IStore = new ViewRunning();
            foreach (var row in LStore)
            {
                IStore.Name = row.Name;
                IStore.NameTh = "สั่งซื้อ";
                IStore.Front = row.Front;
                IStore.Number = row.Number;
                IStore.AutoRun = row.AutoRun;
                List.Add(IStore);
            }

            ViewBag.Branch = _db.MG_Branch.ToList();
            ViewBag.Running = List;
        }
    }
}
