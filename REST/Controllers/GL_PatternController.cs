﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using REST.ApiControllers;
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
    public class GL_PatternController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public GL_PatternController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult Index()
        {
            var _Get = new GetGL_PatternController(_db);
            ViewBag.DataTable = _Get.GetPattern();
            return View();
        }

        public IActionResult FrmPattern(string mode, string id = null)
        {
            
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                var item = new ViewGL_Pattern();
                return View(item);
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                var item = LoadData(id);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmPattern(ViewGL_Pattern info)
        {
            string status = null;
            if (SaveData(info))
            {
                toastrAlert("รูปแบบบัญชี 2", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                status = "success";
            }
            else
            {
                status = "error";
            }
            return Json(new { data = status });
        }

        public Boolean SaveData(ViewGL_Pattern info)
        {
            var item = new GL_Pattern();
            dynamic PatternSub = JsonConvert.DeserializeObject(info.Sub);
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.GL_Pattern.Where(x => x.P_ID == info.P_ID).ToList();
                        if (IsNull.Count > 0)
                        {
                            return false;
                        }
                        else
                        {
                            item.P_ID = info.P_ID;
                            item.P_Name = info.P_Name;
                            item.BookId = info.BookId;
                            item.MenuId = info.MenuId;
                            item.Description = info.Description;
                            /* DATA */
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.GL_Pattern.Add(item);
                            _db.SaveChanges();

                            // Save Template Sub
                            if (PatternSub.Count > 0)
                            {
                                int i = 1;
                                foreach (dynamic result in PatternSub)
                                {
                                    var id = result.AccNo;
                                    var Dr = result.Dr;
                                    var Cr = result.Cr;
                                    var St = result.Status;

                                    var sub = new GL_PatternSub();
                                    sub.P_ID = info.P_ID;
                                    sub.AccNo = id;
                                    if(Dr > 0)
                                    {
                                        sub.Amount = Dr;
                                        sub.DrCr = 1;
                                    }
                                    else
                                    {
                                        sub.Amount = Cr;
                                        sub.DrCr = 2;
                                    }
                                    sub.Status = St;
                                    sub.i = i;

                                    _db.GL_PatternSub.Add(sub);
                                    _db.SaveChanges();

                                    i++;
                                }
                            }
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.GL_Pattern.FirstOrDefault(x => x.P_ID == info.P_ID);
                        item.P_Name = info.P_Name;
                        item.BookId = info.BookId;
                        item.MenuId = info.MenuId;
                        item.Description = item.Description;
                        /* DATA */
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.GL_Pattern.Update(item);
                        _db.SaveChanges();

                        // Save Template Sub
                        if (PatternSub.Count > 0)
                        {
                            var delete = _db.GL_PatternSub.Where(x => x.P_ID == info.P_ID).ToList();
                            _db.GL_PatternSub.RemoveRange(delete);
                            _db.SaveChanges();

                            int i = 1;

                            foreach (dynamic result in PatternSub)
                            {
                                var id = result.AccNo;
                                var Dr = result.Dr;
                                var Cr = result.Cr;
                                var St = result.Status;

                                var sub = new GL_PatternSub();
                                sub.P_ID = info.P_ID;
                                sub.AccNo = id;
                                if (Dr > 0)
                                {
                                    sub.Amount = Dr;
                                    sub.DrCr = 1;
                                }
                                else
                                {
                                    sub.Amount = Cr;
                                    sub.DrCr = 2;
                                }
                                sub.Status = St;
                                sub.i = i;

                                _db.GL_PatternSub.Add(sub);
                                _db.SaveChanges();

                                i++;
                            }
                        }

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
        public IActionResult Delete(ViewGL_Pattern info)
        {
            string status = null;
            try
            {
                _mode = Comp.FormMode.DELETE;

                // Delete Pattern
                var Pattern = _db.GL_Pattern.FirstOrDefault(x => x.P_ID == info.P_ID);

                _db.GL_Pattern.Remove(Pattern);
                _db.SaveChanges();

                // Delete PatternSub
                var PatternSub = _db.GL_PatternSub.Where(x => x.P_ID == info.P_ID);

                _db.GL_PatternSub.RemoveRange(PatternSub);
                _db.SaveChanges();

                toastrAlert("รูปแบบบัญชี 2", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                status = "success";
            }
            catch (Exception)
            {
                status = "error";
            }

            return Json(new { data = status });
        }

        public ViewGL_Pattern LoadData(string id)
        {
            var item = new ViewGL_Pattern();

            var GL_Pattern = _db.GL_Pattern.FirstOrDefault(x => x.P_ID == id);
            item.P_ID = GL_Pattern.P_ID;
            item.P_Name = GL_Pattern.P_Name;
            item.BookId = GL_Pattern.BookId;
            item.MenuId = GL_Pattern.MenuId;
            item.Description = GL_Pattern.Description;
            var _Get = new GetGL_PatternController(_db);
            item.GL_PatternSub = _Get.GetPatternSubById(id);

            return item;
        }

        public void FrmMode()
        {
            if (_mode == Comp.FormMode.ADD)
            {
                ViewData["Disible-delete"] = "disabled";
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "";
                ViewBag.AccountBook = _db.GL_AccountBook.ToList();
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "readonly";
                ViewBag.AccountBook = _db.GL_AccountBook.ToList();
            }
        }
    }
}
