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
using REST.ViewModels;
using REST.ApiControllers;
using static REST.Service.Enums;
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class CD_TableController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public CD_TableController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        // ------

        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _table = new GetCD_TableController(_db);
            ViewBag.DT_Table = _table.Table(branchid);
            return View();
        }

        public IActionResult FrmTable(string mode, string id = null)
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
        public IActionResult FrmTable(ViewTable info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(info, branchid))
                {
                    toastrAlert("โต๊ะ", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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
        public IActionResult Delete(ViewTable info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                var item = _db.CD_Table.FirstOrDefault(x => x.TableId == info.TableId && x.BranchId == branchid);
                _db.CD_Table.Remove(item);
                _db.SaveChanges();

                toastrAlert("โต๊ะ", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                return RedirectToAction("FrmTable", new { mode = "Edit", id = info.TableId });
            }
        }

        public Boolean SaveData(ViewTable info, string branchid)
        {
            var item = new CD_Table();
            var _running = new GetRunningController(_db);
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.CD_Table.Where(x => x.TableId == info.TableId && x.BranchId == branchid).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.TableId = info.TableId;
                            item.TableName = info.TableName;
                            item.Personal = info.Personal;
                            item.Description = info.Description;
                            item.TableST = info.TableST;
                            item.ZoneId = info.ZoneId;
                            /* DATA */
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_Table.Add(item);
                            _db.SaveChanges();

                            // Set Runnig
                            _running.SetRunning(info.ZoneId, info.TableId, branchid);
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.CD_Table.FirstOrDefault(x => x.TableId == info.TableId && x.BranchId == branchid);
                        item.TableName = info.TableName;
                        item.Personal = info.Personal;
                        item.Description = info.Description;
                        item.TableST = info.TableST;
                        /* DATA */
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Table.Update(item);
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
                ViewBag.SL_Zone = _db.CD_Zone.Where(x => x.BranchId == branchid).ToList();
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
                ViewBag.SL_Zone = _db.CD_Zone.Where(x => x.BranchId == branchid).ToList();
            }
        }

        public ViewTable LoadData(string id, string branchid)
        {
            var item = new ViewTable();

            var CD_Table = _db.CD_Table.FirstOrDefault(x => x.TableId == id && x.BranchId == branchid);
            item.ZoneId = CD_Table.ZoneId;
            item.TableId = CD_Table.TableId;
            item.TableName = CD_Table.TableName;
            item.Personal = CD_Table.Personal;
            item.Description = CD_Table.Description;
            item.TableST = CD_Table.TableST;

            return item;
        }        
    }
}
