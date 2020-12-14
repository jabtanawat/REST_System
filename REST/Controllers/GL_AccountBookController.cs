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
    public class GL_AccountBookController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public GL_AccountBookController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult FrmBook(string mode, string id = null)
        {
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                ViewBag.DataTable = _db.GL_AccountBook.ToList();
                return View();
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                ViewBag.DataTable = _db.GL_AccountBook.ToList();
                var item = _db.GL_AccountBook.FirstOrDefault(x => x.BookId == id);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmBook(GL_AccountBook info)
        {
            if (ModelState.IsValid)
            {
                if (SaveData(info))
                {
                    toastrAlert("สมุดบัญชี", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                    return RedirectToAction("FrmBook", new { mode = "Add"});
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
        public IActionResult Delete(GL_AccountBook info)
        {
            try
            {
                // Delete Book
                var item = _db.GL_AccountBook.FirstOrDefault(x => x.BookId == info.BookId);
                _db.GL_AccountBook.Remove(item);
                _db.SaveChanges();

                toastrAlert("สมุดบัญชี", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("FrmBook", new { mode = "Add" });
            }
            catch (Exception)
            {
                Alert("", "Error Data", Enums.NotificationType.warning);
                return RedirectToAction("FrmBook", new { mode = "Edit", id = info.BookId });
            }
        }

        public Boolean SaveData(GL_AccountBook info)
        {
            var item = new GL_AccountBook();
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.GL_AccountBook.Where(x => x.BookId == info.BookId).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            item.BookId = info.BookId;
                            item.BookName = info.BookName;
                            /* DATA */
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.GL_AccountBook.Add(item);
                            _db.SaveChanges();
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.GL_AccountBook.FirstOrDefault(x => x.BookId == info.BookId);
                        item.BookName = info.BookName;
                        /* DATA */
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.GL_AccountBook.Update(item);
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
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "readonly";
            }
        }
    }
}
