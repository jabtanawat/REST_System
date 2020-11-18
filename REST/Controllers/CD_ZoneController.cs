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

namespace REST.Controllers
{
    [Authorize]
    public class CD_ZoneController : BaseController
    {

        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public CD_ZoneController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        //------
        
        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            ViewBag.DT_Zone = _db.CD_Zone.Where(x => x.BranchId == branchid).ToList();
            return View();
        }

        public IActionResult FrmZone(string mode, string id = null)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
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
                var item = LoadData(id, branchid);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmZone(ViewZone info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(info, branchid))
                {
                    toastrAlert("โซนที่นั่ง", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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
        public IActionResult Delete(ViewZone info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                // check zoneid = table 
                var isnull = _db.CD_Table.Where(x => x.ZoneId == info.ZoneId && x.BranchId == branchid).ToList();
                if (isnull.Count > 0)
                {
                    Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                    return RedirectToAction("FrmUnit", new { mode = "Edit", id = info.ZoneId });
                }
                else
                {
                    // Delete Zone
                    var item1 = _db.CD_Zone.FirstOrDefault(x => x.ZoneId == info.ZoneId && x.BranchId == branchid);
                    _db.CD_Zone.Remove(item1);
                    _db.SaveChanges();

                    // Delete Running
                    var item2 = _db.CD_Running.FirstOrDefault(x => x.Name == info.ZoneId && x.BranchId == branchid);
                    _db.CD_Running.Remove(item2);
                    _db.SaveChanges();

                    toastrAlert("โซนที่นั่ง", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                    return RedirectToAction("Index");
                }                
            }
            catch (Exception)
            {
                Alert("", "Error Data", Enums.NotificationType.warning);
                return RedirectToAction("FrmZone", new { mode = "Edit", id = info.ZoneId });
            }
        }

        public Boolean SaveData(ViewZone info, string branchid)
        {
            var item = new CD_Zone();
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.CD_Zone.Where(x => x.ZoneId == info.ZoneId && x.BranchId == branchid).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.ZoneId = info.ZoneId;
                            item.ZoneName = info.ZoneName;
                            item.Description = info.Description;
                            /* DATA */
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_Zone.Add(item);
                            _db.SaveChanges();
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.CD_Zone.FirstOrDefault(x => x.ZoneId == info.ZoneId && x.BranchId == branchid);
                        item.ZoneName = info.ZoneName;
                        item.Description = info.Description;
                        /* DATA */
                        item.BranchId = branchid;
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Zone.Update(item);
                        _db.SaveChanges();

                        break;
                }

                // Save Running
                if (SaveRunning(info, branchid))
                    return true;
                else
                    return false;

            }
            catch (Exception)
            {
                Alert("", "Error Data !", Enums.NotificationType.error);
                return false;
            }
        }

        public Boolean SaveRunning(ViewZone info, string branchid)
        {
            var item = new CD_Running();
            try
            {
                var IsNull = _db.CD_Running.Where(x => x.Name == info.ZoneId && x.BranchId == branchid).ToList();
                if (IsNull.Count > 0)
                {
                    item = _db.CD_Running.FirstOrDefault(x => x.Name == info.ZoneId && x.BranchId == branchid);
                    item.Name = info.ZoneId;
                    item.Front = info.Front;
                    item.Number = info.Number;
                    item.AutoRun = info.AutoRun;
                    /* DATA */
                    item.UpdateUser = User.Identity.Name;
                    item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                    _db.CD_Running.Update(item);
                    _db.SaveChanges();
                }
                else
                {
                    item.Name = info.ZoneId;
                    item.Front = info.Front;
                    item.Number = info.Number;
                    item.AutoRun = info.AutoRun;
                    item.SetDate = null;
                    item.AutoDate = false;
                    /* DATA */
                    item.BranchId = branchid;
                    item.CreateUser = User.Identity.Name;
                    item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                    item.UpdateUser = User.Identity.Name;
                    item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                    _db.CD_Running.Add(item);
                    _db.SaveChanges();
                }

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

        public ViewZone LoadData(string id, string branchid)
        {
            var item = new ViewZone();

            var CD_Zone = _db.CD_Zone.FirstOrDefault(x => x.ZoneId == id && x.BranchId == branchid);
            item.ZoneId = CD_Zone.ZoneId;
            item.ZoneName = CD_Zone.ZoneName;
            item.Description = CD_Zone.Description;
            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == branchid);            
            item.Front = Running.Front;
            item.Number = Running.Number;
            item.AutoRun = Running.AutoRun;

            return item;
        }        
    }
}
