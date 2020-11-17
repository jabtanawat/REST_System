using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using REST.Data;
using REST.Models;
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class CD_TitleController : BaseController
    {

        #region Connect db / Data System

        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;
        public CD_TitleController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            ViewBag.CD_Title = _db.CD_Title.Where(x => x.BranchId == branchid).ToList();
            return View();
        }

        public IActionResult FrmTitle(string mode, string id = null)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                ViewData["Disible-delete"] = "disabled";
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "";
                return View();
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled"; 
                ViewData["Readonly"] = "readonly";
                var Item = _db.CD_Title.FirstOrDefault(x => x.TitleId == id && x.BranchId == branchid);
                return View(Item);
            }
        }

        [HttpPost]
        public IActionResult FrmTitle(CD_Title Info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(Info, branchid))
                {
                    toastrAlert("คำนำหน้า", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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

        public Boolean SaveData(CD_Title Info, string branchid)
        {
            var Item = new CD_Title();
            try
            {            
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.CD_Title.Where(x => x.TitleId == Info.TitleId && x.BranchId == branchid).ToList();
                        if(IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            Item.TitleId = Info.TitleId;
                            Item.TitleName = Info.TitleName;
                            Item.Description = Info.Description;
                            /* DATA */
                            Item.BranchId = branchid;
                            Item.CreateUser = User.Identity.Name;
                            Item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            Item.UpdateUser = User.Identity.Name;
                            Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_Title.Add(Item);
                            _db.SaveChanges();
                        }                        

                        break;

                    case Comp.FormMode.EDIT:
                        Item = _db.CD_Title.FirstOrDefault(x => x.TitleId == Info.TitleId && x.BranchId == branchid);
                        Item.TitleName = Info.TitleName;
                        Item.Description = Info.Description;
                        /* DATA */
                        Item.BranchId = branchid;
                        Item.UpdateUser = User.Identity.Name;
                        Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Title.Update(Item);
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

        [HttpPost]
        public IActionResult Delete(CD_Title Info)
        {

            string branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                var Item = _db.CD_Title.FirstOrDefault(x => x.TitleId == Info.TitleId && x.BranchId == branchid);
                _db.CD_Title.Remove(Item);
                _db.SaveChanges();

                toastrAlert("คำนำหน้า", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                return RedirectToAction("FrmTitle", new { mode = "Edit", id = Info.TitleId });
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
