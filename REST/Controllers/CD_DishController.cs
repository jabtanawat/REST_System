using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class CD_DishController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public CD_DishController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.DT_Dish = _db.CD_Dish.Where(x => x.BranchId == branchid).ToList();
            return View();
        }

        public IActionResult FrmDish(string mode, string id = null)
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
                var item = _db.CD_Dish.FirstOrDefault(x => x.DishId == id && x.BranchId == branchid);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmDish(CD_Dish info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(info, branchid))
                {
                    toastrAlert("เครื่องเคียง", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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
        public IActionResult Delete(CD_Dish info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                // Delete Dish
                var item = _db.CD_Dish.FirstOrDefault(x => x.DishId == info.DishId && x.BranchId == branchid);
                _db.CD_Dish.Remove(item);
                _db.SaveChanges();

                toastrAlert("เครื่องเคียง", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                return RedirectToAction("FrmDish", new { mode = "Edit", id = info.DishId });
            }
        }

        public Boolean SaveData(CD_Dish info, string branchid)
        {
            var item = new CD_Dish();
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.CD_Dish.Where(x => x.DishId == info.DishId && x.BranchId == branchid).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.DishId = info.DishId;
                            item.DishName = info.DishName;
                            item.Description = info.Description;
                            /* DATA */
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_Dish.Add(item);
                            _db.SaveChanges();
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.CD_Dish.FirstOrDefault(x => x.DishId == info.DishId && x.BranchId == branchid);
                        item.DishName = info.DishName;
                        item.Description = info.Description;
                        /* DATA */
                        item.BranchId = branchid;
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Dish.Update(item);
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
