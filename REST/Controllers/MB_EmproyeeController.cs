using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.ApiControllers;
using REST.Data;
using REST.Models;
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class MB_EmproyeeController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public MB_EmproyeeController(DbConnection db)
        {
            _db = db;
        }
        #endregion

        public IActionResult Index()
        {
            var _Get = new GetMB_EmproyeeController(_db);
            ViewBag.Emproyee = _Get.EmproyeeAll();
            return View();
        }

        public IActionResult FrmEmproyee(string mode, string id = null)
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
                var item = LoadData(id);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmEmproyee(ViewMB_Emproyee Info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(Info, branchid))
                {
                    toastrAlert("ทะเบียนพนักงาน", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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
        public IActionResult Delete(ViewMB_Emproyee info)
        {
            try
            {
                var item = _db.MB_Emproyee.FirstOrDefault(x => x.EmproyeeId == info.EmproyeeId);
                _db.MB_Emproyee.Remove(item);
                _db.SaveChanges();

                toastrAlert("ทะเบียนพนักงาน", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                return RedirectToAction("FrmEmproyee", new { mode = "Edit", id = info.EmproyeeId });
            }
        }

        public Boolean SaveData(ViewMB_Emproyee info, string branchid)
        {
            var item = new MB_Emproyee();
            var _Get = new GetRunningController(_db);
            string Doc = null;
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.MB_Emproyee.Where(x => x.EmproyeeId == info.EmproyeeId).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                            return false;
                        }
                        else
                        {
                            /* Get Running */
                            Doc = _Get.Running("Emproyee", null);
                            if (Doc == null)
                                Doc = info.EmproyeeId;

                            item.EmproyeeId = Doc;
                            item.PositionId = info.PositionId;
                            item.DateRegister = Share.FormatDate(info.DateRegister).Date;
                            item.IdCard = info.IdCard;
                            item.Title = info.Title;
                            item.FirstName = info.FirstName;
                            item.LastName = info.LastName;
                            item.DateBirthday = Share.FormatDate(info.DateBirthday).Date;
                            item.Status = info.Status;
                            item.PhoneNumber = info.PhoneNumber;
                            item.Email = info.Email;
                            item.AddrNo = info.AddrNo;
                            item.Moo = info.Moo;
                            item.Road = info.Road;
                            item.Soi = info.Soi;
                            item.Locality = info.Locality;
                            item.District = info.District;
                            item.Province = info.Province;
                            item.ZibCode = info.ZibCode;
                            /* DATA */
                            item.Bch = info.Bch;
                            item.BchName = info.BchName;
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.MB_Emproyee.Add(item);
                            _db.SaveChanges();

                            // Set Running
                            _Get.SetRunning("Emproyee", Doc, null);
                        }

                        break;

                    case Comp.FormMode.EDIT:
                        item = _db.MB_Emproyee.FirstOrDefault(x => x.EmproyeeId == info.EmproyeeId);
                        item.PositionId = info.PositionId;
                        item.IdCard = info.IdCard;
                        item.Title = info.Title;
                        item.FirstName = info.FirstName;
                        item.LastName = info.LastName;
                        item.DateBirthday = Share.FormatDate(info.DateBirthday).Date;
                        item.Status = info.Status;
                        item.PhoneNumber = info.PhoneNumber;
                        item.Email = info.Email;
                        item.AddrNo = info.AddrNo;
                        item.Moo = info.Moo;
                        item.Road = info.Road;
                        item.Soi = info.Soi;
                        item.Locality = info.Locality;
                        item.District = info.District;
                        item.Province = info.Province;
                        item.ZibCode = info.ZibCode;
                        /* DATA */
                        item.Bch = info.Bch;
                        item.BchName = info.BchName;
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.MB_Emproyee.Update(item);
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
                // Running
                var _Get = new GetRunningController(_db);
                string DocRunning = _Get.Running("Emproyee", null);
                if (DocRunning != null)
                {
                    ViewData["Readonly"] = "readonly";
                    ViewData["DocRunning"] = DocRunning;
                }
                else
                {
                    ViewData["Readonly"] = "";
                    ViewData["DocRunning"] = "";
                }
                ViewData["DateNow"] = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.Branch = _db.MG_Branch.ToList();
                ViewBag.Position = _db.CD_Position.ToList();
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
                ViewData["DocRunning"] = "";
                ViewData["DateNow"] = "";
                ViewBag.Branch = _db.MG_Branch.ToList();
                ViewBag.Position = _db.CD_Position.ToList();
            }
        }

        public ViewMB_Emproyee LoadData(string id)
        {
            var item = new ViewMB_Emproyee();

            var info = _db.MB_Emproyee.FirstOrDefault(x => x.EmproyeeId == id);

            item.EmproyeeId = info.EmproyeeId;
            item.PositionId = info.PositionId;
            item.DateRegister = info.DateRegister.ToString("dd/MM/yyyy");
            item.IdCard = info.IdCard;
            item.Title = info.Title;
            item.FirstName = info.FirstName;
            item.LastName = info.LastName;
            item.DateBirthday = info.DateBirthday.ToString("dd/MM/yyyy");
            item.Status = info.Status;
            item.PhoneNumber = info.PhoneNumber;
            item.Email = info.Email;
            item.AddrNo = info.AddrNo;
            item.Moo = info.Moo;
            item.Road = info.Road;
            item.Soi = info.Soi;
            item.Locality = info.Locality;
            item.District = info.District;
            item.Province = info.Province;
            item.ZibCode = info.ZibCode;

            item.Bch = info.Bch;
            item.BchName = info.BchName;

            return item;
        }
    }
}
