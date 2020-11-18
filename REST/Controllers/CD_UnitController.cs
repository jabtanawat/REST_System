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
using static REST.Service.Enums;

namespace REST.Controllers
{
    [Authorize]
    public class CD_UnitController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public CD_UnitController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.DT_Unit = _db.CD_Unit.Where(x => x.BranchId == branchid).ToList();
            return View();
        }

        public IActionResult FrmUnit(string mode, string id = null)
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
                var item = _db.CD_Unit.FirstOrDefault(x => x.UnitId == id && x.BranchId == branchid);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmUnit(CD_Unit info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(info, branchid))
                {
                    toastrAlert("หน่วยนับ", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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
        public IActionResult Delete(CD_Unit info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                // check unitid = staple 
                var isnull = _db.CD_Staple.Where(x => x.UnitId == info.UnitId && x.BranchId == branchid).ToList();
                if(isnull.Count > 0)
                {
                    Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                    return RedirectToAction("FrmUnit", new { mode = "Edit", id = info.UnitId });
                }
                else
                {
                    var item = _db.CD_Unit.FirstOrDefault(x => x.UnitId == info.UnitId && x.BranchId == branchid);
                    _db.CD_Unit.Remove(item);
                    _db.SaveChanges();

                    toastrAlert("หน่วยนับ", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                    return RedirectToAction("Index");
                }                
            }
            catch (Exception)
            {
                Alert("", "Error Data", Enums.NotificationType.error);
                return RedirectToAction("FrmUnit", new { mode = "Edit", id = info.UnitId });
            }
        }

        public Boolean SaveData(CD_Unit info, string branchid)
        {
            var item = new CD_Unit();
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.CD_Unit.Where(x => x.UnitId == info.UnitId && x.BranchId == branchid).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.UnitId = info.UnitId;
                            item.UnitName = info.UnitName;
                            item.Description = info.Description;
                            /* DATA */
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_Unit.Add(item);
                            _db.SaveChanges();
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.CD_Unit.FirstOrDefault(x => x.UnitId == info.UnitId && x.BranchId == branchid);
                        item.UnitName = info.UnitName;
                        item.Description = info.Description;
                        /* DATA */
                        item.BranchId = branchid;
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Unit.Update(item);
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
