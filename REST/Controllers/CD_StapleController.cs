using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.ApiControllers;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;
using static REST.Service.Enums;

namespace REST.Controllers
{
    [Authorize]
    public class CD_StapleController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public CD_StapleController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            var _staple = new GetCD_StapleController(_db);
            ViewBag.DT_Staple = _staple.StapleAll(branchid);
            return View();
        }

        public IActionResult FrmStaple(string mode, string id = null)
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
                var item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == branchid);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmStaple(CD_Staple info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(info, branchid))
                {
                    toastrAlert("วัตถุดิบ", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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
        public IActionResult Delete(CD_Staple info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                // Delete Staple
                var item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == info.StapleId && x.BranchId == branchid);
                _db.CD_Staple.Remove(item);
                _db.SaveChanges();

                toastrAlert("วัตถุดิบ", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                return RedirectToAction("FrmStaple", new { mode = "Edit", id = info.StapleId });
            }
        }

        public Boolean SaveData(CD_Staple info, string branchid)
        {
            var item = new CD_Staple();
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.CD_Staple.Where(x => x.StapleId == info.StapleId && x.BranchId == branchid).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.StapleId = info.StapleId;
                            item.StapleName = info.StapleName;
                            item.Amount = info.Amount;
                            item.UnitId = info.UnitId;
                            /* DATA */
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_Staple.Add(item);
                            _db.SaveChanges();
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == info.StapleId && x.BranchId == branchid);
                        item.StapleName = info.StapleName;
                        item.Amount = info.Amount;
                        item.UnitId = info.UnitId;
                        /* DATA */
                        item.BranchId = branchid;
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Staple.Update(item);
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
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            if (_mode == Comp.FormMode.ADD)
            {
                ViewData["Disible-delete"] = "disabled";
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "";
                ViewBag.SL_Unit = _db.CD_Unit.Where(x => x.BranchId == branchid).ToList();
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
                ViewBag.SL_Unit = _db.CD_Unit.Where(x => x.BranchId == branchid).ToList();
            }
        }        
    }
}
