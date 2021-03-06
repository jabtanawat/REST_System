﻿using System;
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
    public class CD_GroupFoodController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public CD_GroupFoodController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            ViewBag.DT_GroupFood = _db.CD_GroupFood.ToList();
            return View();
        }

        public IActionResult FrmGroupFood(string mode, string id = null)
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
        public IActionResult FrmGroupFood(ViewGroupFood info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(info, branchid))
                {
                    toastrAlert("กลุ่มอาหาร", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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
        public IActionResult Delete(ViewGroupFood info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                // Delete Zone
                var item1 = _db.CD_GroupFood.FirstOrDefault(x => x.GroupFoodId == info.GroupFoodId && x.BranchId == branchid);
                _db.CD_GroupFood.Remove(item1);
                _db.SaveChanges();

                // Delete Running
                var item2 = _db.CD_Running.FirstOrDefault(x => x.Name == info.GroupFoodId && x.BranchId == branchid);
                _db.CD_Running.Remove(item2);
                _db.SaveChanges();

                toastrAlert("กลุ่มอาหาร", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                return RedirectToAction("FrmGroupFood", new { mode = "Edit", id = info.GroupFoodId });
            }
        }

        public Boolean SaveData(ViewGroupFood info, string branchid)
        {
            var item = new CD_GroupFood();
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.CD_GroupFood.Where(x => x.GroupFoodId == info.GroupFoodId).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.GroupFoodId = info.GroupFoodId;
                            item.GroupFoodName = info.GroupFoodName;
                            item.Description = info.Description;
                            /* DATA */
                            item.Bch = info.Bch;
                            item.BchName = info.BchName;
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_GroupFood.Add(item);
                            _db.SaveChanges();
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.CD_GroupFood.FirstOrDefault(x => x.GroupFoodId == info.GroupFoodId && x.BranchId == branchid);
                        item.GroupFoodName = info.GroupFoodName;
                        item.Description = info.Description;
                        /* DATA */
                        item.Bch = info.Bch;
                        item.BchName = info.BchName;
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_GroupFood.Update(item);
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

        public Boolean SaveRunning(ViewGroupFood info, string branchid)
        {
            var item = new CD_Running();
            try
            {
                var IsNull = _db.CD_Running.Where(x => x.Name == info.GroupFoodId && x.BranchId == branchid).ToList();
                if (IsNull.Count > 0)
                {
                    item = _db.CD_Running.FirstOrDefault(x => x.Name == info.GroupFoodId && x.BranchId == branchid);
                    item.Name = info.GroupFoodId;
                    item.Front = info.Front;
                    item.Number = info.Number;
                    item.AutoRun = info.AutoRun;
                    /* DATA */
                    item.BchName = info.BchName;
                    item.UpdateUser = User.Identity.Name;
                    item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                    _db.CD_Running.Update(item);
                    _db.SaveChanges();
                }
                else
                {
                    item.Name = info.GroupFoodId;
                    item.Front = info.Front;
                    item.Number = info.Number;
                    item.AutoRun = info.AutoRun;
                    item.SetDate = null;
                    item.AutoDate = false;
                    /* DATA */
                    item.BchName = info.BchName;
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
                ViewBag.Branch = _db.MG_Branch.ToList();
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
                ViewBag.Branch = _db.MG_Branch.ToList();
            }
        }

        public ViewGroupFood LoadData(string id, string branchid)
        {
            var item = new ViewGroupFood();

            var CD_GroupFood = _db.CD_GroupFood.FirstOrDefault(x => x.GroupFoodId == id && x.BranchId == branchid);
            item.GroupFoodId = CD_GroupFood.GroupFoodId;
            item.GroupFoodName = CD_GroupFood.GroupFoodName;
            item.Description = CD_GroupFood.Description;      
            
            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == id);
            item.Front = Running.Front;
            item.Number = Running.Number;
            item.AutoRun = Running.AutoRun;
            // --- Data
            item.Bch = CD_GroupFood.Bch;
            item.BchName = CD_GroupFood.BchName;

            return item;
        }        
    }
}
