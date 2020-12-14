using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Controllers
{
    [Authorize]
    public class GL_AccountChartController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public GL_AccountChartController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult FrmAccountChart(string mode, string id = null)
        {
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                ViewBag.DataTable = _db.GL_AccountChart.ToList();
                return View();
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                ViewBag.DataTable = _db.GL_AccountChart.ToList();
                var item = LoadData(id);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmAccountChart(ViewGL_AccountChart info)
        {
            if (ModelState.IsValid)
            {
                if (SaveData(info))
                {
                    toastrAlert("ผังบัญชี", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                    return RedirectToAction("FrmAccountChart", new { mode = "Add" });
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
        public IActionResult Delete(ViewGL_AccountChart info)
        {
            var _AccNo = info.Control + info.AccNo;
            try
            {
                // Delete AccountChart
                var item = _db.GL_AccountChart.FirstOrDefault(x => x.AccNo == _AccNo);
                _db.GL_AccountChart.Remove(item);
                _db.SaveChanges();

                toastrAlert("ผังบัญชี", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("FrmAccountChart", new { mode = "Add" });
            }
            catch (Exception)
            {
                Alert("", "Error Data", Enums.NotificationType.warning);
                return RedirectToAction("FrmAccountChart", new { mode = "Edit", id = _AccNo });
            }
        }

        public Boolean SaveData(ViewGL_AccountChart info)
        {
            var item = new GL_AccountChart();
            var _AccNo = info.Control + info.AccNo;
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.GL_AccountChart.Where(x => x.AccNo == _AccNo).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.Type = Share.FormatInteger(info.Type);
                            item.AccNo = _AccNo;
                            item.AccName = info.AccName;
                            item.DrCr = Share.FormatInteger(info.DrCr);
                            /* DATA */
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.GL_AccountChart.Add(item);
                            _db.SaveChanges();
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.GL_AccountChart.FirstOrDefault(x => x.AccNo == _AccNo);
                        item.AccName = info.AccName;
                        item.DrCr = Share.FormatInteger(info.DrCr);
                        /* DATA */
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.GL_AccountChart.Update(item);
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

        public ViewGL_AccountChart LoadData(string id)
        {
            var item = new ViewGL_AccountChart();

            var GL_AccountChart = _db.GL_AccountChart.FirstOrDefault(x => x.AccNo == id);
            item.Type = Share.FormatString(GL_AccountChart.Type);
            item.Control = GL_AccountChart.AccNo.Substring(0,1);
            item.AccNo = GL_AccountChart.AccNo.Substring(1);
            item.AccName = GL_AccountChart.AccName;
            item.DrCr = Share.FormatString(GL_AccountChart.DrCr);

            return item;
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
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "readonly";
            }
        }
    }
}
