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
    public class MB_SupplierController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public MB_SupplierController(DbConnection db)
        {
            _db = db;
        }
        #endregion

        public IActionResult Index()
        {
            var _Get = new GetMB_SupplierController(_db);
            ViewBag.Supplier = _Get.SupplierAll();
            return View();
        }

        public IActionResult FrmSupplier(string mode, string id = null)
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
        public IActionResult FrmSupplier(ViewMB_Supplier Info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(Info, branchid))
                {
                    toastrAlert("Supplier", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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
        public IActionResult Delete(ViewMB_Supplier info)
        {
            try
            {
                var item = _db.MB_Supplier.FirstOrDefault(x => x.SupplierId == info.SupplierId);
                _db.MB_Supplier.Remove(item);
                _db.SaveChanges();

                toastrAlert("Supplier", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                return RedirectToAction("FrmSupplier", new { mode = "Edit", id = info.SupplierId });
            }
        }

        public Boolean SaveData(ViewMB_Supplier info, string branchid)
        {
            var item = new MB_Supplier();
            var _Get = new GetRunningController(_db);
            string Doc = null;
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.MB_Supplier.Where(x => x.SupplierId == info.SupplierId).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                            return false;
                        }
                        else
                        {
                            /* Get Running */
                            Doc = _Get.Running("Supplier", null);
                            if (Doc == null)
                                Doc = info.SupplierId;

                            item.SupplierId = Doc;
                            item.DateRegister = Share.FormatDate(info.DateRegister).Date;
                            item.TaxNumber = info.TaxNumber;
                            item.IdCard = info.IdCard;
                            item.Name = info.Name;
                            item.PhoneNumber1 = info.PhoneNumber1;
                            item.PhoneNumber2 = info.PhoneNumber2;
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

                            _db.MB_Supplier.Add(item);
                            _db.SaveChanges();

                            // Set Running
                            _Get.SetRunning("Supplier", Doc, null);
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.MB_Supplier.FirstOrDefault(x => x.SupplierId == info.SupplierId);
                        item.TaxNumber = info.TaxNumber;
                        item.IdCard = info.IdCard;
                        item.Name = info.Name;
                        item.PhoneNumber1 = info.PhoneNumber1;
                        item.PhoneNumber2 = info.PhoneNumber2;
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

                        _db.MB_Supplier.Update(item);
                        _db.SaveChanges();

                        break;
                }

                return true;
            }
            catch (Exception ex)
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
                string DocRunning = _Get.Running("Supplier", null);
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

        public ViewMB_Supplier LoadData(string id)
        {
            var item = new ViewMB_Supplier();

            var info = _db.MB_Supplier.FirstOrDefault(x => x.SupplierId == id);

            item.SupplierId = info.SupplierId;
            item.DateRegister = info.DateRegister.ToString("dd/MM/yyyy");
            item.TaxNumber = info.TaxNumber;
            item.IdCard = info.IdCard;
            item.Name = info.Name;
            item.PhoneNumber1 = info.PhoneNumber1;
            item.PhoneNumber2 = info.PhoneNumber2;
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
