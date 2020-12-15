using Microsoft.AspNetCore.Authorization;
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
    public class GL_TemplateController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public GL_TemplateController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult Index()
        {
            var _Get = new GetGL_TemplateController(_db);
            ViewBag.DataTable = _Get.GetTemplate();
            return View();
        }

        public IActionResult FrmTemplate(string mode, string id = null)
        {
            
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                var item = new ViewGL_Template();
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
        public IActionResult FrmTemplate(ViewGL_Template info)
        {
            string status = null;
            if (SaveData(info))
            {
                toastrAlert("ผังบัญชี", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                status = "success";
            }
            else
            {
                status = "error";
            }
            return Json(new { data = status });
        }

        public Boolean SaveData(ViewGL_Template info)
        {
            var item = new GL_Template();
            dynamic TemplateSub = JsonConvert.DeserializeObject(info.Sub);
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.GL_Template.Where(x => x.M_ID == info.M_ID).ToList();
                        if (IsNull.Count > 0)
                        {
                            return false;
                        }
                        else
                        {
                            item.M_ID = info.M_ID;
                            item.M_Name = info.M_Name;
                            item.M_DesCription = info.M_DesCription;
                            item.BookId = info.BookId;
                            item.M_PType = Share.FormatInteger(info.M_PType);
                            /* DATA */
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.GL_Template.Add(item);
                            _db.SaveChanges();

                            // Save Template Sub
                            if (TemplateSub.Count > 0)
                            {
                                int i = 1;
                                foreach (dynamic result in TemplateSub)
                                {
                                    var id = result.AccNo;
                                    var Dr = result.Dr;
                                    var Cr = result.Cr;

                                    var sub = new GL_TemplateSub();
                                    sub.M_ID = info.M_ID;
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
                                    sub.i = i;

                                    _db.GL_TemplateSub.Add(sub);
                                    _db.SaveChanges();

                                    i++;
                                }
                            }
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.GL_Template.FirstOrDefault(x => x.M_ID == info.M_ID);
                        item.M_Name = info.M_Name;
                        item.M_DesCription = info.M_DesCription;
                        item.BookId = info.BookId;
                        item.M_PType = Share.FormatInteger(info.M_PType);
                        /* DATA */
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.GL_Template.Update(item);
                        _db.SaveChanges();

                        // Save Template Sub
                        if (TemplateSub.Count > 0)
                        {
                            var delete = _db.GL_TemplateSub.Where(x => x.M_ID == info.M_ID).ToList();
                            _db.GL_TemplateSub.RemoveRange(delete);
                            _db.SaveChanges();

                            int i = 1;

                            foreach (dynamic result in TemplateSub)
                            {
                                var id = result.AccNo;
                                var Dr = result.Dr;
                                var Cr = result.Cr;

                                var sub = new GL_TemplateSub();
                                sub.M_ID = info.M_ID;
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
                                sub.i = i;

                                _db.GL_TemplateSub.Add(sub);
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
        public IActionResult Delete(ViewGL_Template info)
        {
            string status = null;
            try
            {

                _mode = Comp.FormMode.DELETE;

                // Delete Template
                var Template = _db.GL_Template.FirstOrDefault(x => x.M_ID == info.M_ID);

                _db.GL_Template.Remove(Template);
                _db.SaveChanges();

                // Delete TemplateSub
                var TemplateSub = _db.GL_TemplateSub.Where(x => x.M_ID == info.M_ID);

                _db.GL_TemplateSub.RemoveRange(TemplateSub);
                _db.SaveChanges();

                toastrAlert("ผังบัญชี", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                status = "success";
            }
            catch (Exception)
            {
                status = "error";
            }

            return Json(new { data = status });
        }

        public ViewGL_Template LoadData(string id)
        {
            var item = new ViewGL_Template();

            var GL_Template = _db.GL_Template.FirstOrDefault(x => x.M_ID == id);
            item.M_ID = GL_Template.M_ID;
            item.M_Name = GL_Template.M_Name;
            item.M_DesCription = GL_Template.M_DesCription;
            item.BookId = GL_Template.BookId;
            item.M_PType = Share.FormatString(GL_Template.M_PType);
            var _Get = new GetGL_TemplateController(_db);
            item.GL_TemplateSub = _Get.GetTemplateSubById(id);

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
