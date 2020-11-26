using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Data;
using REST.Models;
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class MG_BranchController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public MG_BranchController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult Index()
        {
            ViewBag.DT_Branch = _db.MG_Branch.ToList();
            return View();
        }

        public IActionResult FrmBranch(string mode, string id = null)
        {
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                return View();
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                var item = _db.MG_Branch.FirstOrDefault(x => x.BranchId == id);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmBranch(MG_Branch info)
        {
            if (ModelState.IsValid)
            {
                if (SaveData(info))
                {
                    toastrAlert("สาขา", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    FrmMode();
                    return View();
                }
            }
            else
            {
                FrmMode();
                Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", Enums.NotificationType.error);
                return View();
            }
        }

        [HttpPost]
        public IActionResult Delete(MG_Branch info)
        {
            try
            {
                var item = _db.MG_Branch.FirstOrDefault(x => x.BranchId == info.BranchId);
                _db.MG_Branch.Remove(item);
                _db.SaveChanges();

                var Running = _db.CD_Running.Where(x => x.BranchId == info.BranchId).ToList();
                _db.CD_Running.RemoveRange(Running);
                _db.SaveChanges();

                toastrAlert("สาขา", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "Error Data", Enums.NotificationType.error);
                return RedirectToAction("FrmBranch", new { mode = "Edit", id = info.BranchId });
            }
        }

        public Boolean SaveData(MG_Branch info)
        {
            var item = new MG_Branch();
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.MG_Branch.Where(x => x.BranchId == info.BranchId).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.BranchId = info.BranchId;
                            item.BranchName = info.BranchName;
                            item.St = info.St;
                            item.AddrNo = info.AddrNo;
                            item.Moo = info.Moo;
                            item.Road = info.Road;
                            item.Soi = info.Soi;
                            item.Locality = info.Locality;
                            item.District = info.District;
                            item.Province = info.Province;
                            item.ZibCode = info.ZibCode;
                            /* DATA */
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.MG_Branch.Add(item);
                            _db.SaveChanges();

                            // เพิ่ม Running ประจำสาขา
                            AddRunning(info);
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.MG_Branch.FirstOrDefault(x => x.BranchId == info.BranchId);
                        item.BranchName = info.BranchName;
                        item.St = info.St;
                        item.AddrNo = info.AddrNo;
                        item.Moo = info.Moo;
                        item.Road = info.Road;
                        item.Soi = info.Soi;
                        item.Locality = info.Locality;
                        item.District = info.District;
                        item.Province = info.Province;
                        item.ZibCode = info.ZibCode;
                        /* DATA */
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.MG_Branch.Update(item);
                        _db.SaveChanges();

                        break;
                }

                return true;
            }
            catch (Exception)
            {
                Alert("", "Error Data !", Enums.NotificationType.error);
                return false;
            }
        }

        public Boolean AddRunning(MG_Branch info)
        {
            var item = new CD_Running();
            try
            {
                item.Name = "Emproyee";
                item.Front = "EM";
                item.Number = "0000";
                item.AutoRun = true;
                item.SetDate = null;
                item.AutoDate = false;
                /* DATA */
                item.BchName = null;
                item.BranchId = info.BranchId;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Running.Add(item);
                _db.SaveChanges();

                item.Name = "Member";
                item.Front = "MB";
                item.Number = "0000";
                item.AutoRun = true;
                item.SetDate = null;
                item.AutoDate = false;
                /* DATA */
                item.BchName = null;
                item.BranchId = info.BranchId;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Running.Add(item);
                _db.SaveChanges();

                item.Name = "Supplier";
                item.Front = "SP";
                item.Number = "0000";
                item.AutoRun = true;
                item.SetDate = null;
                item.AutoDate = false;
                /* DATA */
                item.BchName = null;
                item.BranchId = info.BranchId;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Running.Add(item);
                _db.SaveChanges();

                item.Name = "Payment";
                item.Front = "PY";
                item.Number = "0000";
                item.AutoRun = true;
                item.SetDate = null;
                item.AutoDate = false;
                /* DATA */
                item.BchName = null;
                item.BranchId = info.BranchId;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Running.Add(item);
                _db.SaveChanges();

                item.Name = "Order";
                item.Front = "OR";
                item.Number = "0000";
                item.AutoRun = true;
                item.SetDate = null;
                item.AutoDate = false;
                /* DATA */
                item.BchName = null;
                item.BranchId = info.BranchId;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Running.Add(item);
                _db.SaveChanges();

                item.Name = "Store";
                item.Front = "ST";
                item.Number = "0000";
                item.AutoRun = true;
                item.SetDate = null;
                item.AutoDate = false;
                /* DATA */
                item.BchName = null;
                item.BranchId = info.BranchId;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Running.Add(item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void FrmMode()
        {
            if (_mode == Comp.FormMode.ADD)
            {
                ViewData["Disible-delete"] = "disabled";
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "";
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
            }
        }
    }
}
