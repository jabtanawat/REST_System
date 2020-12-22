using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Data;
using REST.Models;
using REST.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Controllers
{
    [Authorize]
    public class CD_UserRoleGroupController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public CD_UserRoleGroupController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult Index()
        {
            ViewBag.Data = _db.CD_UserRoleGroup.ToList();
            return View();
        }

        public IActionResult FrmUserRoleGroup(string mode, string id = null)
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
                var item = _db.CD_UserRoleGroup.FirstOrDefault(x => x.RoleGroupId == id);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmUserRoleGroup(CD_UserRoleGroup info)
        {
            if (ModelState.IsValid)
            {
                if (SaveData(info))
                {
                    toastrAlert("กลุ่มผู้ใช้งาน", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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
        public IActionResult Delete(CD_UserRoleGroup info)
        {
            try
            {
                // check dishid = food 
                //var isnull = _db.CD_Food.Where(x => x.DishId == info.DishId).ToList();
                //if (isnull.Count > 0)
                //{
                //    Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                //    return RedirectToAction("FrmDish", new { mode = "Edit", id = info.DishId });
                //}
                //else
                //{
                    // Delete UserRoleGroup
                    var item = _db.CD_UserRoleGroup.FirstOrDefault(x => x.RoleGroupId == info.RoleGroupId);
                    _db.CD_UserRoleGroup.Remove(item);
                    _db.SaveChanges();

                    toastrAlert("กลุ่มผู้ใช้งาน", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                    return RedirectToAction("Index");
                //}
            }
            catch (Exception)
            {
                Alert("", "Error Data", Enums.NotificationType.warning);
                return RedirectToAction("FrmUserRoleGroup", new { mode = "Edit", id = info.RoleGroupId });
            }
        }

        public Boolean SaveData(CD_UserRoleGroup info)
        {
            var item = new CD_UserRoleGroup();
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.CD_UserRoleGroup.Where(x => x.RoleGroupId == info.RoleGroupId).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.RoleGroupId = info.RoleGroupId;
                            item.RoleGroupName = info.RoleGroupName;
                            item.Active = info.Active;
                            item.SystemConfig = info.SystemConfig;
                            item.SaveData = info.SaveData;
                            item.ApproveData = info.ApproveData;
                            item.ViewData = info.ViewData;
                            item.PrintReport = info.PrintReport;
                            item.Description = info.Description;
                            /* DATA */
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_UserRoleGroup.Add(item);
                            _db.SaveChanges();
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.CD_UserRoleGroup.FirstOrDefault(x => x.RoleGroupId == info.RoleGroupId);
                        item.RoleGroupName = info.RoleGroupName;
                        item.Active = info.Active;
                        item.SystemConfig = info.SystemConfig;
                        item.SaveData = info.SaveData;
                        item.ApproveData = info.ApproveData;
                        item.ViewData = info.ViewData;
                        item.PrintReport = info.PrintReport;
                        item.Description = info.Description;
                        /* DATA */
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_UserRoleGroup.Update(item);
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
                ViewData["Disible-Edit"] = "disabled";
                ViewData["Hidden-Save"] = "";
                ViewData["Readonly"] = "";
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-Edit"] = "";
                ViewData["Hidden-Save"] = "hidden";
                ViewData["Readonly"] = "readonly";
            }
        }
    }
}
